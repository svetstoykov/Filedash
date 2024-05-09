using System.Collections.Immutable;
using System.Text;
using Filedash.Domain.Common;
using Filedash.Domain.Models;

namespace Filedash.Domain.Interfaces;

public interface IUploadedFilesManagementService
{
    Task<DataResult<UploadedFileDetails>> UploadEncodedStringAsync(
        string content,
        string fileName,
        Encoding encoding,
        CancellationToken cancellationToken = default);
    
    Task<DataResult<UploadedFileDetails>> UploadFileStreamAsync(
        Stream fileStream,
        long? fileLength,
        string fileNameWithExtension,
        CancellationToken cancellationToken = default);

    Task<DataResult<IImmutableList<UploadedFileDetails>>> ListAllFilesAsync(
        CancellationToken cancellationToken = default);

    Task<Result> DeleteFileAsync(
        Guid id, 
        CancellationToken cancellationToken = default);
}