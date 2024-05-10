namespace Filedash.Domain.Models;

public record UploadedFileDetails
{
    public Guid Id { get; set; }
    
    public string FullFileName { get; set; }
    
    public long ContentLength { get; set; }
    
    public DateTime CreatedDateUtc { get; set; }
}