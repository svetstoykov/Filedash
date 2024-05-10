namespace Filedash.Domain.Interfaces;

public interface IFileSettings
{
    string TemporaryFileFolderName { get; set; }
    
    int BinaryEncodedTextMaxLength { get; set; }
}