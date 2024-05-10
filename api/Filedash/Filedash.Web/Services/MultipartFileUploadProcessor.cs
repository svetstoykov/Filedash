using System.Collections.Immutable;
using System.Text;
using Filedash.Domain.Common;
using Filedash.Domain.Interfaces;
using Filedash.Domain.Models;
using Filedash.Web.Helpers;
using Filedash.Web.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Filedash.Web.Services;

public class MultipartFileUploadProcessor : IMultipartFileUploadProcessor
{
    private readonly FormOptions _defaultFormOptions;
    private readonly IUploadedFilesManagementService _uploadedFilesManagementService;

    public MultipartFileUploadProcessor(IUploadedFilesManagementService uploadedFilesManagementService)
    {
        _uploadedFilesManagementService = uploadedFilesManagementService;
        _defaultFormOptions = new FormOptions();
    }

    public async Task<IImmutableList<Result<UploadedFileDetails>>> ProcessMultipartFileUploadsAsync(
        HttpRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!MultipartRequestHelper.IsMultipartContentType(request.ContentType))
        {
            throw new Exception($"Expected a multipart request, but got {request.ContentType}");
        }

        var boundary = MultipartRequestHelper
            .GetBoundary(
                MediaTypeHeaderValue.Parse(request.ContentType),
                _defaultFormOptions.MultipartBoundaryLengthLimit);

        var reader = new MultipartReader(boundary, request.Body);

        var section = await TryReadNextSectionAsync(cancellationToken, reader);

        var resultSet = new List<Result<UploadedFileDetails>>();

        while (section != null)
        {
            var hasContentDispositionHeader = ContentDispositionHeaderValue
                .TryParse(section.ContentDisposition, out var contentDispositionHeader);

            if (!hasContentDispositionHeader)
            {
                section = await TryReadNextSectionAsync(cancellationToken, reader);
                
                continue;
            }

            if (contentDispositionHeader.IsFileDisposition())
            {
                var fileSection = section.AsFileSection();

                if (fileSection?.FileStream == null
                    || IsFilestreamEmpty(fileSection.FileStream))
                {
                    resultSet.Add(Result<UploadedFileDetails>
                        .Failure("File section and/or file stream  is null or empty!"));

                    section = await TryReadNextSectionAsync(cancellationToken, reader);

                    continue;
                }

                var result = await _uploadedFilesManagementService.UploadFileStreamAsync(
                    fileSection.FileStream,
                    fileSection.FileName,
                    cancellationToken);

                resultSet.Add(result);
            }
            else if (contentDispositionHeader.IsFormDisposition())
            {
                var key = HeaderUtilities.RemoveQuotes(contentDispositionHeader.Name);
                var encoding = GetEncoding(section);
                if (encoding == null)
                {
                    throw new NullReferenceException("Null encoding");
                }

                using var streamReader = new StreamReader(
                    section.Body, encoding);

                var value = await streamReader.ReadToEndAsync(cancellationToken);

                var result = await _uploadedFilesManagementService.UploadEncodedStringAsync(
                    value, key.Value, encoding, cancellationToken);

                resultSet.Add(result);
            }


            section = await TryReadNextSectionAsync(cancellationToken, reader);
        }

        return resultSet.ToImmutableList();
    }

    private static async Task<MultipartSection> TryReadNextSectionAsync(CancellationToken cancellationToken,
        MultipartReader reader)
    {
        MultipartSection section;
        try
        {
            section = await reader.ReadNextSectionAsync(cancellationToken);
        }
        catch (IOException)
        {
            section = null;
        }

        return section;
    }

    private static bool IsFilestreamEmpty(Stream fileSectionFileStream)
    {
        if (fileSectionFileStream.ReadByte() == -1)
        {
            return true;
        }

        fileSectionFileStream.Position = 0;

        return false;
    }

    private static Encoding GetEncoding(MultipartSection section)
    {
        var hasMediaTypeHeader = MediaTypeHeaderValue
            .TryParse(section.ContentType, out var mediaType);

        // UTF-7 is insecure and should not be honored. UTF-8 will succeed in most cases.
        if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
        {
            return Encoding.UTF8;
        }

        return mediaType.Encoding;
    }
}