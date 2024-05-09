using System.Collections.Immutable;
using System.Text;
using Filedash.Domain.Common;
using Filedash.Domain.Models;

namespace Filedash.Domain.Interfaces;

public interface IUploadedFilesManagementService
{
    Task<Result<UploadedFileDetails>> UploadEncodedStringAsync(
        string content,
        string fileName,
        Encoding encoding,
        CancellationToken cancellationToken = default);
    
    Task<Result<UploadedFileDetails>> UploadFileStreamAsync(
        Stream fileStream,
        long? fileLength,
        string fileNameWithExtension,
        CancellationToken cancellationToken = default);

    Task<Result<IImmutableList<UploadedFileDetails>>> ListAllFilesAsync(
        CancellationToken cancellationToken = default);

    Task<Result> DeleteFileAsync(
        Guid id, 
        CancellationToken cancellationToken = default);
}