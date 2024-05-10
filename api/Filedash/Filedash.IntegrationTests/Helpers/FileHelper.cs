using AutoFixture;
using Filedash.Domain.Models;

namespace Filedash.IntegrationTests.Helpers;

public static class FileHelper
{
    private static readonly string[] TestFileExtension = [".jpg", ".exe", ".png", ".xls", "gif"];
    private static readonly Fixture Fixture = new();
    private static readonly Random Random = new();

    private const string EmptyFile = "file-example_TXT_0Kb.txt";
    private const string SmallFile = "file_example_JPG_100kB.jpg";
    private const string MediumFile = "file-example_PDF_500_kB.pdf";
    private const string LargeJpgFile = "file_example_JPG_1MB.jpg";
    private const string LargeMp3File = "file_example_MP3_5MG.mp3";
    private const string LargeMp4File = "file_example_MP4_1920_18MG.mp4";
    
    public static async Task<(string, byte[])> GetEmptyFileAsync()
        => (EmptyFile, await ReadFileBytesAsync(SmallFile));
    
    public static async Task<(string, byte[])> GetSmallFileAsync()
        => (SmallFile, await ReadFileBytesAsync(SmallFile));

    public static async Task<(string, byte[])> GetMediumFileAsync()
        => (MediumFile, await ReadFileBytesAsync(MediumFile));

    public static async Task<(string, byte[])> GetLargeJpgFileAsync()
        => (LargeJpgFile, await ReadFileBytesAsync(LargeJpgFile));

    public static async Task<(string, byte[])> GetLargeMp3FileAsync()
        => (LargeMp3File, await ReadFileBytesAsync(LargeMp3File));

    public static async Task<(string, byte[])> GetLargeMp4FileAsync()
        => (LargeMp4File, await ReadFileBytesAsync(LargeMp4File));

    
    public static IEnumerable<UploadedFile> GetUploadedFiles(int count = 5)
    {
        var files = new List<UploadedFile>();

        for (var i = 0; i < count; i++)
        {
            var file = GetUploadedFile();

            files.Add(file);
        }

        return files;
    }

    public static UploadedFile GetUploadedFile()
        => Fixture.Build<UploadedFile>()
            .With(g => g.Extension, TestFileExtension[Random.Next(0, TestFileExtension.Length)])
            .Create();
    
    private static async Task<byte[]> ReadFileBytesAsync(string fileName)
    {
        var path = $@"{AppDomain.CurrentDomain.BaseDirectory}\Content\{fileName}";

        var fileBytesAsync = await File.ReadAllBytesAsync(path);

        return fileBytesAsync;
    }
}