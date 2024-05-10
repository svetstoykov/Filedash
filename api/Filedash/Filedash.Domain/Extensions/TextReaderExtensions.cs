using System.Text;

namespace Filedash.Domain.Extensions;

public static class TextReaderExtensions
{
    public static async Task<string> ReadLinesWithLimit(this TextReader streamReader, int maxLength)
    {
        var sb = new StringBuilder();
        const int buffersize = 1;
        
        var buffer = new char[buffersize];

        while ((await streamReader.ReadAsync(buffer, 0, buffersize)) > 0)
        {
            var c = buffer[0];

            if (c is '\r' or '\n')
            {
                return sb.ToString();
            }

            sb.Append(c);
            if (sb.Length > maxLength)
            {
                throw new InvalidOperationException($"Max binary encoded text length exceeded! Limit: {maxLength}");
            }
        }

        return sb.ToString();
    }
}