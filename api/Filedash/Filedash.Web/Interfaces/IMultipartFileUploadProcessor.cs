namespace Filedash.Web.Interfaces;

public interface IMultipartFileUploadProcessor
{
    Task ProcessMultipartFileUploadAsync(HttpRequest request, CancellationToken cancellationToken = default);
}