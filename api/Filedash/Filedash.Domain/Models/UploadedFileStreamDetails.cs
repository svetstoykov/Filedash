namespace Filedash.Domain.Models;

public class UploadedFileStreamDetails
{
    public string FullFileName { get; set; }
    
    public Stream ContentStream { get; set; }
}