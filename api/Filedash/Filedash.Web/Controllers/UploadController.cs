using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Filedash.StartUp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UploadController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> UploadFile()
    {
        // Handle large file upload using MultipartReader
        var reader = new MultipartReader("boundary", HttpContext.Request.Body);
        var section = await reader.ReadNextSectionAsync();
        while (section != null)
        {
            var contentDisposition = section.GetContentDispositionHeader();
            if (contentDisposition != null && contentDisposition.FileName != null)
            {
                // Process file section
                // Save to disk or do other processing
            }
            section = await reader.ReadNextSectionAsync();
        }

        return Ok("File uploaded successfully");
    }    
}