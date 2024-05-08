﻿namespace Filedash.Domain.Models;

public class UploadFile
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    public string Extension { get; set; }

    public byte[] Content { get; set; }

    public DateTime CreatedDate { get; set; }
}