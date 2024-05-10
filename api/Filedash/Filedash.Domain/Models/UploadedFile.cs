using System.Text;

namespace Filedash.Domain.Models;

public class UploadedFile
{
    private Guid _id;
    private string _name;
    private string _extension;
    private DateTime _createdDateUtc;

    public UploadedFile(
        Guid id,
        string name, 
        string extension, 
        DateTime createdDateUtc,
        long contentLength = default,
        byte[] content = default,
        string encodingType = default)
    {
        Id = id;
        Name = name;
        Extension = extension;
        CreatedDateUtc = createdDateUtc;
        ContentLength = contentLength;
        Content = content;
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

    public string Name
    {
        get => _name;
        private set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("File name cannot be null or empty!");
            }

            _name = value;
        }
    }

    public string Extension
    {
        get => _extension;
        private set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("File extension cannot be null or empty!");
            }

            _extension = value;
        }
    }

    public DateTime CreatedDateUtc
    {
        get => _createdDateUtc;
        private set
        {
            if (value == default)
            {
                throw new ArgumentException("Invalid created date!");
            }

            _createdDateUtc = value;
        }
    }

    public string EncodingType { get; private set; }
    
    public byte[] Content { get; private set; }
    
    public long ContentLength { get; private set; }

    public void SetContentLength(long contentLength)
    {
        if (contentLength == default)
        {
            throw new InvalidOperationException(
                "Invalid content length! The file may be empty.");
        }

        ContentLength = contentLength;
    }

    public static UploadedFile New(
        string name, string extension, long contentLength = default, byte[] content = default, string encodingType = default) 
        => new(Guid.NewGuid(), name, extension, DateTime.UtcNow, contentLength, content, encodingType);
}
