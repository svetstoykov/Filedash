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

    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    [DisableRequestSizeLimit]
    [DisableFormValueModelBinding]
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(CancellationToken cancellationToken)
    {
        var results = await _multipartFileUploadProcessor
            .ProcessMultipartFileUploadsAsync(Request, cancellationToken);

        return results.All(r => !r.IsSuccessful)
            ? BadRequest("Failed to upload all files!")
            : Ok(results);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var uploadResult = await _uploadedFilesManagementService
            .DeleteFileAsync(id, cancellationToken);

        return uploadResult.IsSuccessful ? Ok() : BadRequest();
    }

    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> ListAllUploadedFiles(CancellationToken cancellationToken)
    {
        var uploadedFileDetails = await _uploadedFilesManagementService
            .ListAllFilesAsync(cancellationToken);

        return uploadedFileDetails.IsSuccessful ? Ok(uploadedFileDetails) : BadRequest(uploadedFileDetails);
    }
}