namespace Filedash.Domain.Models;

public record UploadedFileDetails
{
    private Guid _id;
    private string _fullFileName;

    public UploadedFileDetails(
        Guid id, 
        string fullFileName, 
        long contentLength,
        DateTime createdDateUtc,
        string encodingType = default)
    {
        Id = id;
        FullFileName = fullFileName;
        ContentLength = contentLength;
        CreatedDateUtc = createdDateUtc;
        EncodingType = encodingType;
    }

    public Guid Id
    {
        get => _id;
        private set
        {
            if (value == default)
            {
                throw new ArgumentException("Id is invalid!");
            }

            _id = value;
        }
    }

    public string FullFileName
    {
        get => _fullFileName;
        private set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("File name cannot be null or empty!");
            }

            _fullFileName = value;
        }
    }

    public long ContentLength { get; private set; }

    public DateTime CreatedDateUtc { get; private set; }
    
    public string EncodingType { get; private set; }
    
    public static UploadedFileDetails MapFromUploadedFile(UploadedFile uploadedFile)
        => new(
            uploadedFile.Id,
            $"{uploadedFile.Name}{uploadedFile.Extension}",
            uploadedFile.ContentLength,
            uploadedFile.CreatedDateUtc,
            uploadedFile.EncodingType);
}