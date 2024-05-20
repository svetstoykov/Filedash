using Microsoft.AspNetCore.Mvc;

namespace Filedash.Web.Results;

public class TempFileStreamResult : FileStreamResult
{
    private readonly string _fullPathToFile;

    public TempFileStreamResult(
        Stream fileStream,
        string contentType,
        string fullPathToFile,
        string fileDownloadName = default) : base(fileStream,
        contentType)
    {
        _fullPathToFile = fullPathToFile;
        FileDownloadName = fileDownloadName ?? Path.GetFileName(fullPathToFile);
    }

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        try
        {
            await base.ExecuteResultAsync(context);
        }
        finally
        {
            File.Delete(_fullPathToFile);
        }
    }
}