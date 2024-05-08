using Microsoft.EntityFrameworkCore;
using File = Filedash.Domain.Models.File;

namespace Filedash.Infrastructure.Models;

public partial class FiledashDbContext : DbContext
{
    public FiledashDbContext(DbContextOptions<FiledashDbContext> dbContextOptions) 
        : base(dbContextOptions)
    {
    }
    
    public virtual DbSet<File> Files { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<File>(entity =>
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