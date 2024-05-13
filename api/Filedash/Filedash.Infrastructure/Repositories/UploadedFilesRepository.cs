using System.Data;
using Filedash.Domain.Interfaces;
using Filedash.Domain.Models;
using Filedash.Infrastructure.DbContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Filedash.Infrastructure.Repositories;

public class UploadedFilesRepository : IUploadedFilesRepository
{
    private readonly FiledashDbContext _context;

    public UploadedFilesRepository(FiledashDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SaveUploadedFileAsync(UploadedFile uploadedFile, CancellationToken cancellationToken = default)
    {
        _context.Add(uploadedFile);

        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> StreamUploadedFileAsync(
        UploadedFile file,
        Stream fileContentStream,
        CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@id", SqlDbType.UniqueIdentifier) {Value = file.Id},
            new("@name", SqlDbType.NVarChar) {Value = file.Name},
            new("@extension", SqlDbType.NVarChar) {Value = file.Extension},
            new("@content", SqlDbType.Binary, -1) {Value = fileContentStream},
            new("@contentLength", SqlDbType.BigInt, -1) {Value = file.ContentLength},
            new("@createdDate", SqlDbType.DateTime2) {Value = file.CreatedDateUtc}
        };

        var result = await _context.Database.ExecuteSqlRawAsync(
            "INSERT INTO [dbo].[UploadedFiles] ([Id], [Name], [Extension], [Content], [ContentLength], [CreatedDateUtc]) " +
            "VALUES (@id, @name, @extension, @content, @contentLength, @createdDate);",
            parameters,
            cancellationToken: cancellationToken);

        return result > 0;
    }

    public async Task<bool> DoesFileNameWithExtensionExistAsync(
        string fileName,
        string extension,
        CancellationToken cancellationToken = default)
        => await _context.UploadedFiles.AnyAsync(f =>
                f.Name.ToLower().Equals(fileName.ToLower())
                && f.Extension.ToLower().Equals(extension.ToLower()),
            cancellationToken: cancellationToken);

    public async Task<bool> DeleteFileAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.UploadedFiles
            .Where(f => f.Id == id)
            .ExecuteDeleteAsync(cancellationToken) > 0;

    public async Task<IEnumerable<UploadedFileDetails>> ListAllUploadedFiles(
        CancellationToken cancellationToken = default)
        => await _context.UploadedFiles
            .Select(f => new UploadedFile(f.Id, f.Name, f.Extension, f.CreatedDateUtc,f.ContentLength, null, f.EncodingType))
            .Select(f => UploadedFileDetails.MapFromUploadedFile(f))
            .ToListAsync(cancellationToken);
}