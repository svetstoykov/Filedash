namespace Filedash.Domain.Models;

public class UploadedFile
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }

    public string Extension { get; set; }

    public byte[] Content { get; set; }
    
    public long ContentLength { get; set; }

    public DateTime CreatedDateUtc { get; set; }
}
