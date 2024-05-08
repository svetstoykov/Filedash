using System.Data;
using Filedash.Domain.Interfaces;
using Filedash.Infrastructure.DbContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Filedash.Infrastructure.Services;

public class UploadedFilesRepository : IUploadedFilesRepository
{
    private readonly FiledashDbContext _context;

    public UploadedFilesRepository(FiledashDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SaveUploadedFileAsync(
        Stream fileStream, 
        string fileName, 
        string extension, 
        DateTime createdDate,
        CancellationToken cancellationToken = default)
    {
        var parameters = new List<SqlParameter>
        {
            new("@id", SqlDbType.UniqueIdentifier) {Value = Guid.NewGuid()},
            new("@name", SqlDbType.NVarChar) {Value = fileName},
            new("@extension", SqlDbType.NVarChar) {Value = extension},
            new("@bindata", SqlDbType.Binary, -1) {Value = fileStream},
            new("@createdDate", SqlDbType.DateTime2) {Value = createdDate}
        };

        var result = await _context.Database.ExecuteSqlRawAsync(
            "INSERT INTO [dbo].[UploadedFiles] ([Id], [Name], [Extension], [Content], [CreatedDate]) VALUES (@id, @name, @extension, @bindata, @createdDate);",
            parameters,
            cancellationToken: cancellationToken);

        return result > 0;
    }
}