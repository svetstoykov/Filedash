using System.Collections.Immutable;
using Filedash.Domain.Common;
using Filedash.Domain.Models;

namespace Filedash.Web.Interfaces;

public interface IMultipartFileUploadProcessor
{
    Task<IImmutableList<Result<UploadedFileDetails>>> ProcessMultipartFileUploadsAsync(
        HttpRequest request, CancellationToken cancellationToken = default);
}