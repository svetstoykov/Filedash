using Filedash.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Filedash.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IMultipartFileUploadProcessor _multipartFileUploadProcessor;

    public FilesController(IMultipartFileUploadProcessor multipartFileUploadProcessor)
    {
        _multipartFileUploadProcessor = multipartFileUploadProcessor;
    }

    [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
    [DisableRequestSizeLimit]
    [Consumes("multipart/form-data")]
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(CancellationToken cancellationToken)
    {
        await _multipartFileUploadProcessor
            .ProcessMultipartFileUploadAsync(Request, cancellationToken);
        
        return Ok("File uploaded successfully");
    }
}