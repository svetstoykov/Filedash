using System.Collections.Immutable;
using System.Text;
using Filedash.Domain.Common;
using Filedash.Domain.Models;

namespace Filedash.Domain.Interfaces;

public interface IUploadedFilesManagementService
{
    Task<Result<UploadedFileDetails>> UploadBinaryEncodedTextStreamAsync(
        Stream fileStream,
        string fileNameWithExtension,
        Encoding encoding,
        CancellationToken cancellationToken = default);
    
    Task<Result<UploadedFileDetails>> UploadFileStreamAsync(
        Stream fileStream,
        string fileNameWithExtension,
        CancellationToken cancellationToken = default);

    Task<Result<IImmutableList<UploadedFileDetails>>> ListAllFilesAsync(
        CancellationToken cancellationToken = default);

    Task<Result<(string, string)>> DownloadFileToLocalPathAsync(
        Guid id, 
        CancellationToken cancellationToken = default);

    Task<Result> DeleteFileAsync(
        Guid id, 
        CancellationToken cancellationToken = default);
}