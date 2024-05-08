using Filedash.Domain.Interfaces;
using Filedash.Web.Extensions;
using Filedash.Web.Helpers;
using Filedash.Web.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Filedash.Web.Services;

public class MultipartFileUploadProcessor : IMultipartFileUploadProcessor
{
    private readonly FormOptions _defaultFormOptions;
    private readonly IFileManagementService _fileManagementService;

    public MultipartFileUploadProcessor(IFileManagementService fileManagementService)
    {
        _fileManagementService = fileManagementService;
        _defaultFormOptions = new FormOptions();
    }

    public async Task ProcessMultipartFileUploadAsync(HttpRequest request,
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

        var section = await TryReadNextSectionAsync(reader, cancellationToken);

        while (section != null)
        {
            var hasContentDispositionHeader = ContentDispositionHeaderValue
                .TryParse(section.ContentDisposition, out var contentDispositionHeader);

            if (!hasContentDispositionHeader)
            {
                continue;
            }

            if (contentDispositionHeader.IsFileDisposition())
            {
                var fileSection = section.AsFileSection();

                if (fileSection?.FileStream == null)
                {
                    throw new InvalidOperationException();
                }

                await _fileManagementService.UploadFileStreamAsync(
                    fileSection.FileStream,
                    request.ContentLength,
                    fileSection.FileName,
                    cancellationToken);
            }
            else if (contentDispositionHeader.IsFormDisposition())
            {
                var key = HeaderUtilities.RemoveQuotes(contentDispositionHeader.Name);
                var encoding = section.GetEncoding();
                if (encoding == null)
                {
                    throw new NullReferenceException("Null encoding");
                }

                using var streamReader = new StreamReader(
                    section.Body,
                    encoding,
                    detectEncodingFromByteOrderMarks: true,
                    bufferSize: 1024,
                    leaveOpen: true);

                var value = await streamReader.ReadToEndAsync(cancellationToken);

                await _fileManagementService.UploadEncodedStringAsync(
                    value, key.Value, encoding, cancellationToken);
            }

            section = await TryReadNextSectionAsync(reader, cancellationToken);
        }
    }

    private static async Task<MultipartSection> TryReadNextSectionAsync(MultipartReader reader, CancellationToken cancellationToken)
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
}