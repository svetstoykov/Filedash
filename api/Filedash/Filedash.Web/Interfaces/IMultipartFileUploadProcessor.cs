using System.Collections.Immutable;
using Filedash.Domain.Common;
using Filedash.Domain.Models;

namespace Filedash.Web.Interfaces;

public interface IMultipartFileUploadProcessor
{
    Task<IImmutableList<DataResult<UploadedFileDetails>>> ProcessMultipartFileUploadsAsync(HttpRequest request, CancellationToken cancellationToken = default);
}