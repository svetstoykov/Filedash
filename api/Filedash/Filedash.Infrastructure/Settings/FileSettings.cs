using Filedash.Domain.Interfaces;

namespace Filedash.Infrastructure.Settings;

public class FileSettings : IFileSettings
{
    public string TemporaryFileFolderName { get; set; }
    
    public int BinaryEncodedTextMaxLength { get; set; }
    
    public int FileDeleteDelayAfterDownloadInMinutes { get; set; }
}