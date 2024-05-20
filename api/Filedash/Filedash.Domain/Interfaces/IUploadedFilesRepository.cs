using System.Collections.Immutable;
using System.Linq.Expressions;
using Filedash.Domain.Models;

namespace Filedash.Domain.Interfaces;

public interface IUploadedFilesRepository
{
    Task<bool> SaveUploadedFileAsync(
        UploadedFile uploadedFile, 
        CancellationToken cancellationToken = default);
    
    Task<bool> StreamUploadedFileAsync(
        UploadedFile file,
        Stream fileContentStream,
        CancellationToken cancellationToken = default);

    Task<bool> DoesFileNameWithExtensionExistAsync(
        string fileName,
        string extension,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteFileAsync(
        Guid id, 
        CancellationToken cancellationToken = default);

    Task<UploadedFileDetails> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    Task CopyFileStreamToLocalPathByIdAsync(
        Guid id, string localPath, CancellationToken cancellationToken = default);

    Task<IEnumerable<UploadedFileDetails>> ListAllUploadedFiles(
        CancellationToken cancellationToken = default);
}