namespace Filedash.Domain.Interfaces;

public interface IUploadedFilesRepository
{
    Task<bool> SaveUploadedFileAsync(
        Stream fileStream, 
        string fileName, 
        string extension, 
        DateTime createdDate,
        CancellationToken cancellationToken = default);
}