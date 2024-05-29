using Filedash.Domain.Common;
using Filedash.Domain.Interfaces;
using Filedash.Web.Attributes;
using Filedash.Web.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace Filedash.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IMultipartFileUploadProcessor _multipartFileUploadProcessor;
    private readonly IUploadedFilesManagementService _uploadedFilesManagementService;
    private readonly IFileSettings _fileSettings;

    public FilesController(
        IMultipartFileUploadProcessor multipartFileUploadProcessor,
        IUploadedFilesManagementService uploadedFilesManagementService, 
        IFileSettings fileSettings)
    {
        _multipartFileUploadProcessor = multipartFileUploadProcessor;
        _uploadedFilesManagementService = uploadedFilesManagementService;
        _fileSettings = fileSettings;
    }

    [DisableFormValueModelBinding]
    [DisableRequestSizeLimit]
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

    [HttpGet]
    [Route("download/{id}")]
    public async Task<IActionResult> DownloadFile(Guid id, CancellationToken cancellationToken)
    {
        var result = await _uploadedFilesManagementService
            .DownloadFileToLocalPathAsync(id, cancellationToken);
        
        if (!result.IsSuccessful)
        {
            return BadRequest(result);
        }

        var (path, fileName) = result.Data;

        ScheduleFileDeleteOnResponseCompleted(path);
        
        var fileStream = System.IO.File.Open(path, FileMode.Open);
        
        return File(fileStream, "application/octet-stream", fileName);
    }

    private void ScheduleFileDeleteOnResponseCompleted(string path)
        => Response.OnCompleted(() =>
        {
            BackgroundJob.Schedule<IUploadedFilesManagementService>(
                s => s.DeleteFileAsync(path, default),
                TimeSpan.FromMinutes(_fileSettings.FileDeleteDelayAfterDownloadInMinutes));
            
            return Task.CompletedTask;
        });

    private IActionResult ProcessServiceResult(Result result)
        => result.IsSuccessful
            ? Ok(result)
            : BadRequest(result);
}