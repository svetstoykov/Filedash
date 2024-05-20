using System.Data;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Filedash.Domain.Interfaces;
using Filedash.Domain.Models;
using Filedash.Infrastructure.DbContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Filedash.Infrastructure.Repositories;

public class UploadedFilesRepository : IUploadedFilesRepository
{
    private readonly FiledashDbContext _context;
    private readonly IMapper _mapper;

    public UploadedFilesRepository(
        FiledashDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<bool> SaveUploadedFileAsync(UploadedFile uploadedFile,
        CancellationToken cancellationToken = default)
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

    public async Task<UploadedFileDetails> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.UploadedFiles
            .ProjectTo<UploadedFileDetails>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken: cancellationToken);

    public async Task CopyFileStreamToLocalPathByIdAsync(
        Guid id, string localPath, CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(
            _context.Database.GetDbConnection().ConnectionString);

        await connection.OpenAsync(cancellationToken);

        await using var command = new SqlCommand(
            "SELECT [Content] FROM [dbo].[UploadedFiles] WHERE [id]=@id", connection);

        command.Parameters.Add(new SqlParameter("@id", SqlDbType.UniqueIdentifier) {Value = id});

        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess, cancellationToken);

        if (!await reader.ReadAsync(cancellationToken) || await reader.IsDBNullAsync(default, cancellationToken))
        {
            throw new InvalidOperationException();
        }

        await using var fileStream = File.Open(localPath, FileMode.Create, FileAccess.Write);

        await using var data = reader.GetStream(0);

        await data.CopyToAsync(fileStream, cancellationToken);
    }

    public async Task<IEnumerable<UploadedFileDetails>> ListAllUploadedFiles(
        CancellationToken cancellationToken = default)
        => await _context.UploadedFiles
            .ProjectTo<UploadedFileDetails>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
}