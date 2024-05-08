using Filedash.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Filedash.Infrastructure.DbContext;

public partial class FiledashDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public FiledashDbContext(DbContextOptions<FiledashDbContext> dbContextOptions) 
        : base(dbContextOptions)
    {
    }
    
    public virtual DbSet<UploadFile> UploadedFiles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UploadFile>(entity =>
        {
            entity.Property(e => e.Extension)
                .HasMaxLength(10)
                .IsRequired();
            
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsRequired();

            entity
                .Property(e => e.Content)
                .IsRequired();
        });
    }
}