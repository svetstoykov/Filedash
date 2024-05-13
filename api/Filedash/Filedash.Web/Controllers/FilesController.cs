using Filedash.Domain.Common;
using Filedash.Domain.Interfaces;
using Filedash.Web.Attributes;
using Filedash.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Filedash.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IMultipartFileUploadProcessor _multipartFileUploadProcessor;
    private readonly IUploadedFilesManagementService _uploadedFilesManagementService;

    public FilesController(
        IMultipartFileUploadProcessor multipartFileUploadProcessor,
        IUploadedFilesManagementService uploadedFilesManagementService)
    {
        _multipartFileUploadProcessor = multipartFileUploadProcessor;
        _uploadedFilesManagementService = uploadedFilesManagementService;
    }

    [DisableFormValueModelBinding]
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(CancellationToken cancellationToken)
    {
        var results = await _multipartFileUploadProcessor
            .ProcessMultipartFileUploadsAsync(Request, cancellationToken);

        return results.All(r => !r.IsSuccessful)
            ? BadRequest(results)
            : Ok(results);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _uploadedFilesManagementService
            .DeleteFileAsync(id, cancellationToken);

        return ProcessServiceResult(result);
    }

    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> ListAllUploadedFiles(CancellationToken cancellationToken)
    {
        var result = await _uploadedFilesManagementService
            .ListAllFilesAsync(cancellationToken);

        return ProcessServiceResult(result);
    }

    private IActionResult ProcessServiceResult(Result result)
        => result.IsSuccessful
            ? Ok(result)
            : BadRequest(result);
}