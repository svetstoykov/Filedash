namespace Filedash.Domain.Models;

public class UploadedFileDetails
{
    public Guid Id { get; set; }
    
    public string FullFileName { get; set; }
    
    public long ContentLength { get; set; }
}