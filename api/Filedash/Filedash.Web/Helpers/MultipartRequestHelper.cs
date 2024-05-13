using Microsoft.Net.Http.Headers;

namespace Filedash.Web.Helpers;

public static class MultipartRequestHelper
{
    public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
    {
        var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;
        if (string.IsNullOrWhiteSpace(boundary))
            throw new InvalidDataException("Missing content-type boundary.");

        if (boundary.Length > lengthLimit)
            throw new InvalidDataException($"Multipart boundary length limit {lengthLimit} exceeded.");

        return boundary;
    }

    public static bool IsMultipartContentType(string contentType) 
        => !string.IsNullOrEmpty(contentType) && contentType.Contains("multipart/", StringComparison.OrdinalIgnoreCase);
}