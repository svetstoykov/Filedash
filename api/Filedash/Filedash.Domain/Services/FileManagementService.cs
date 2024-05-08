using System.Collections.Immutable;
using System.Text;
using Filedash.Domain.Common;
using Filedash.Domain.Interfaces;
using Filedash.Domain.Models;

namespace Filedash.Domain.Services;

public class FileManagementService : IFileManagementService
{
    private readonly IUploadedFilesRepository _uploadedFilesRepository;

    public FileManagementService(IUploadedFilesRepository uploadedFilesRepository)
    {
        _uploadedFilesRepository = uploadedFilesRepository;
    }

    public async Task<Result> UploadEncodedStringAsync(string content, string fileName, Encoding encoding, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> UploadFileStreamAsync(Stream fileStream, long? fileLength, string fullFileName,
        CancellationToken cancellationToken = default)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            return Result.Failure(
                "File stream cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(fullFileName))
        {
            return Result.Failure(
                "File name cannot be null or empty.");
        }

        var extension = Path.GetExtension(fullFileName);
        var name = Path.GetFileName(fullFileName);

        if (string.IsNullOrEmpty(extension))
        {
            return Result.Failure(
                "Cannot determine file extension from the provided file name.");
        }

        if (fileLength.HasValue && fileStream.Length != fileLength)
        {
            return Result.Failure(
                "File length does not match the provided length.");
        }

        await _uploadedFilesRepository
            .SaveUploadedFileAsync(fileStream, name, extension, DateTime.UtcNow, cancellationToken: cancellationToken);

        return Result.Success();
    }

    public async Task<DataResult<ImmutableList<UploadedFile>>> ListAllFilesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> DeleteFileAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}