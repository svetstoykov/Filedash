using System.Collections.Immutable;
using System.Text;
using Filedash.Domain.Common;
using Filedash.Domain.Interfaces;
using Filedash.Domain.Models;

namespace Filedash.Domain.Services;

public class UploadedFilesManagementService : IUploadedFilesManagementService
{
    private readonly IUploadedFilesRepository _uploadedFilesRepository;

    public UploadedFilesManagementService(IUploadedFilesRepository uploadedFilesRepository)
    {
        _uploadedFilesRepository = uploadedFilesRepository;
    }

    public async Task<DataResult<UploadedFileDetails>> UploadEncodedStringAsync(
        string content,
        string fileName,
        Encoding encoding,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<DataResult<UploadedFileDetails>> UploadFileStreamAsync(
        Stream fileStream,
        long? fileLength,
        string fileNameWithExtension,
        CancellationToken cancellationToken = default)
    {
        var (fileName, extension) = ExtractFileInfo(fileNameWithExtension);

        var validationResult = ValidateUploadInput(
            fileStream, fileName, extension, fileLength);

        if (!validationResult.IsSuccessful)
        {
            return DataResult<UploadedFileDetails>.Failure();
        }

        var fileExists = await _uploadedFilesRepository
            .DoesFileNameWithExtensionExistAsync(fileName, extension, cancellationToken);

        if (fileExists)
        {
            return DataResult<UploadedFileDetails>
                .Failure($"A file with name '{fileName}' and extension '{extension}' already exists!");
        }

        var uploadedFile = CreateUploadedFile(
            fileName, extension, fileLength!.Value);

        var createdId = await _uploadedFilesRepository
            .StreamUploadedFileAsync(uploadedFile, fileStream, cancellationToken: cancellationToken);

        return DataResult<UploadedFileDetails>.Success(
            new UploadedFileDetails
        {
            Id = createdId,
            ContentLength = uploadedFile.ContentLength,
            CreatedDateUtc = uploadedFile.CreatedDateUtc,
            FullFileName = fileNameWithExtension
        });
    }

    public async Task<DataResult<IImmutableList<UploadedFileDetails>>> ListAllFilesAsync(
        CancellationToken cancellationToken = default)
    {
        var allFilesList = await _uploadedFilesRepository
            .ListAllUploadedFiles(cancellationToken);

        return DataResult<IImmutableList<UploadedFileDetails>>
            .Success(allFilesList.ToImmutableList());
    }

    public async Task<Result> DeleteFileAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var isDeleteSuccessful = await _uploadedFilesRepository
            .DeleteFileAsync(id, cancellationToken);

        return isDeleteSuccessful 
            ? Result.Success() 
            : Result.Failure("Delete operation failed!");
    }

    private static Result ValidateUploadInput(Stream fileStream, string fileName, string extension, long? fileLength)
    {
        if (fileStream == null)
        {
            return Result
                .Failure("File stream cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return Result
                .Failure("File name cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(extension))
        {
            return Result
                .Failure("File does not have valid file extension.");
        }

        if (fileLength is not > (long) default)
        {
            return Result
                .Failure("Invalid file content length!");
        }

        return Result.Success();
    }

    private static (string name, string extension) ExtractFileInfo(string fullFileName)
    {
        var extension = Path.GetExtension(fullFileName);

        var name = Path.GetFileNameWithoutExtension(fullFileName);

        return (name, extension);
    }

    private static UploadedFile CreateUploadedFile(string name, string extension, long contentLength)
        => new()
        {
            Name = name,
            Extension = extension,
            ContentLength = contentLength,
            CreatedDateUtc = DateTime.UtcNow
        };
}