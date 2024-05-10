using AutoFixture;
using Filedash.Domain.Models;
using Filedash.Infrastructure.DbContext;

namespace Filedash.IntegrationTests.Helpers;

public static class DatabaseHelper
{
    public static async Task InsertAsync(FiledashDbContext testDb, params UploadedFile[] files)
    {
        testDb.UploadedFiles.AddRange(files);
        
        await testDb.SaveChangesAsync();
    }

    public static async Task CleanDbAsync(FiledashDbContext db)
    {
        db.UploadedFiles.RemoveRange(db.UploadedFiles);

        await db.SaveChangesAsync();
    }
}