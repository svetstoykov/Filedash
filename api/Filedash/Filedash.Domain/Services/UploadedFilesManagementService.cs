using System.Collections.Immutable;
using System.Text;
using Filedash.Domain.Common;
using Filedash.Domain.Extensions;
using Filedash.Domain.Interfaces;
using Filedash.Domain.Models;

namespace Filedash.Domain.Services;

public class UploadedFilesManagementService : IUploadedFilesManagementService
{
    private readonly IUploadedFilesRepository _uploadedFilesRepository;
    private readonly IFileSettings _fileSettings;

    public UploadedFilesManagementService(
        IUploadedFilesRepository uploadedFilesRepository,
        IFileSettings fileSettings)
    {
        _uploadedFilesRepository = uploadedFilesRepository;
        _fileSettings = fileSettings;
    }

    public async Task<Result<UploadedFileDetails>> UploadBinaryEncodedTextStreamAsync(
        Stream fileStream,
        string fileNameWithExtension,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
        using var streamReader = new StreamReader(
            fileStream, encoding);

        var content = await streamReader.ReadLinesWithLimit(
            _fileSettings.BinaryEncodedTextMaxLength);

        var contentBuffer = new byte[content.Length];

        if (!Convert.TryFromBase64String(content, contentBuffer, out var contentLength))
        {
            return Result<UploadedFileDetails>
                .Failure("Input is not valid Base64 string. Only Base64 binary-to-text encoding is allowed!");
        }

        var (fileName, extension) = ExtractFileInfo(fileNameWithExtension);

        var uploadedFile = UploadedFile.New(
            fileName, extension, contentLength, contentBuffer, encoding.BodyName);

        var fileExists = await _uploadedFilesRepository
            .DoesFileNameWithExtensionExistAsync(fileName, extension, cancellationToken);

        if (fileExists)
        {
            return Result<UploadedFileDetails>.Failure(
                $"A file with name '{fileName}' and extension '{extension}' already exists!");
        }

        var isSuccessful = await _uploadedFilesRepository
            .SaveUploadedFileAsync(uploadedFile, cancellationToken);

        return isSuccessful
            ? Result<UploadedFileDetails>.Success(
                UploadedFileDetails.MapFromUploadedFile(uploadedFile))
            : Result<UploadedFileDetails>.Failure(
                $"Failed to save file: '{fileNameWithExtension}'!");
    }

    public async Task<Result<UploadedFileDetails>> UploadFileStreamAsync(
        Stream fileStream,
        string fileNameWithExtension,
        CancellationToken cancellationToken = default)
    {
        var (fileName, extension) = ExtractFileInfo(fileNameWithExtension);

        var uploadedFile = UploadedFile.New(fileName, extension);

        var fileExists = await _uploadedFilesRepository
            .DoesFileNameWithExtensionExistAsync(fileName, extension, cancellationToken);

        if (fileExists)
        {
            return Result<UploadedFileDetails>.Failure(
                $"A file with name '{fileName}' and extension '{extension}' already exists!");
        }

        var tempFilePath = PrepareTempFilePath(fileNameWithExtension);

        Result<UploadedFileDetails> result;
        try
        {
            await using var temporaryFileStream = File.Create(tempFilePath);

            await fileStream.CopyToAsync(temporaryFileStream, cancellationToken);

            temporaryFileStream.Seek(0, SeekOrigin.Begin);

            var contentLength = new FileInfo(tempFilePath).Length;

            uploadedFile.SetContentLength(contentLength);

            await _uploadedFilesRepository
                .StreamUploadedFileAsync(
                    uploadedFile,
                    temporaryFileStream,
                    cancellationToken: cancellationToken);

            result = Result<UploadedFileDetails>.Success(
                UploadedFileDetails.MapFromUploadedFile(uploadedFile));
        }
        catch (Exception ex)
        {
            result = Result<UploadedFileDetails>
                .Failure(ex.Message);
        }
        finally
        {
            File.Delete(tempFilePath);
        }

        return result;
    }

    public async Task<Result<IImmutableList<UploadedFileDetails>>> ListAllFilesAsync(
        CancellationToken cancellationToken = default)
    {
        var allFilesList = await _uploadedFilesRepository
            .ListAllUploadedFiles(cancellationToken);

        var sortedFiles = allFilesList
            .OrderByDescending(f => f.CreatedDateUtc)
            .ToImmutableList();

        return Result<IImmutableList<UploadedFileDetails>>
            .Success(sortedFiles);
    }

    public async Task<Result<(string, string)>> DownloadFileToLocalPathAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var file = await _uploadedFilesRepository
            .GetByIdAsync(id, cancellationToken);

        if (file == null)
        {
            return Result<(string, string)>
                .Failure("File does not exist!");
        }
        
        var path = PrepareTempFilePath(Guid.NewGuid().ToString());

        await _uploadedFilesRepository
            .CopyFileStreamToLocalPathByIdAsync(id, path, cancellationToken);

        return Result<(string, string)>
            .Success((path, file.FullFileName));
    }

    public async Task<Result> DeleteFileAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var isDeleteSuccessful = await _uploadedFilesRepository
            .DeleteFileAsync(id, cancellationToken);

        return isDeleteSuccessful
            ? Result.Success()
            : Result.Failure("Delete operation failed!");
    }

    private string PrepareTempFilePath(string fileNameWithExtension)
    {
        var tempFileFolder = GetTempFileFolder();

        Directory.CreateDirectory(tempFileFolder);

        return Path.Combine(tempFileFolder, fileNameWithExtension);
    }

    private string GetTempFileFolder()
        => Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            _fileSettings.TemporaryFileFolderName);
    
    private static (string name, string extension) ExtractFileInfo(string fullFileName)
    {
        var extension = Path.GetExtension(fullFileName);

        var name = Path.GetFileNameWithoutExtension(fullFileName);

        return (name, extension);
    }
}