using System.Collections.Immutable;
using System.Text;
using Filedash.Domain.Common;
using Filedash.Domain.Models;

namespace Filedash.Domain.Interfaces;

public interface IFileManagementService
{
    Task<Result> UploadEncodedStringAsync(
        string content,
        string fileName,
        Encoding encoding,
        CancellationToken cancellationToken);
    
    Task<Result> UploadFileStreamAsync(
        Stream fileStream,
        long? fileLength,
        string fullFileName,
        CancellationToken cancellationToken = default);

    Task<DataResult<ImmutableList<UploadedFile>>> ListAllFilesAsync(
        CancellationToken cancellationToken = default);

    Task<Result> DeleteFileAsync(
        string id, CancellationToken cancellationToken = default);
}