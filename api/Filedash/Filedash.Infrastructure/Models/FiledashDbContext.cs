using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            entity.Property(e => e.Extension).HasMaxLength(10);
            entity.Property(e => e.Name).HasMaxLength(50);
        });
    }
}