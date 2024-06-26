﻿using Filedash.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Filedash.Infrastructure.DbContext;

public partial class FiledashDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public FiledashDbContext(DbContextOptions<FiledashDbContext> dbContextOptions) 
        : base(dbContextOptions)
    {
    }
    
    public virtual DbSet<UploadedFile> UploadedFiles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UploadedFile>(entity =>
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

            entity.Property(e => e.EncodingType)
                .HasMaxLength(50)
                .IsRequired(false);

            entity
                .HasIndex(e => new {e.Name, e.Extension})
                .HasDatabaseName("UQ_UploadedFile_Name_Extension")
                .IsUnique();
        });
    }
}