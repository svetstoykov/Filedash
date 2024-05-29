using System.Text;

namespace Filedash.Domain.Extensions;

public static class TextReaderExtensions
{
    public static async Task<string> ReadLinesWithLimitAsync(this StreamReader streamReader, int maxLength)
    {
        var sb = new StringBuilder();
        const int bufferSize = 1000;

        var buffer = new char[bufferSize];

        int bytesRead;
        while ((bytesRead = await streamReader.ReadAsync(buffer, 0, bufferSize)) > 0)
        {
            if (bytesRead == default)
            {
                return sb.ToString();
            }

            var charsToAppend = bytesRead < bufferSize 
                ? buffer[..bytesRead] 
                : buffer;

            sb.Append(charsToAppend);

            GuardAgainstMaxLengthReached(sb, maxLength);

            var lastChar = buffer[bytesRead - 1];
            if (lastChar is '\r' or '\n')
            {
                return sb.ToString();
            }
        }

        return sb.ToString();
    }

    private static void GuardAgainstMaxLengthReached(StringBuilder builder, int maxLength)
    {
        if (builder.Length > maxLength)
        {
            throw new InvalidOperationException($"Max binary encoded text length exceeded! Limit: {maxLength}");
        }
    }
}