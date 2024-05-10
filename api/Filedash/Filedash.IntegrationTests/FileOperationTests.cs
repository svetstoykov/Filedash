using System.Net;
using System.Text.Json;
using Filedash.Domain.Common;
using Filedash.Domain.Models;
using Filedash.Infrastructure.DbContext;
using Filedash.IntegrationTests.Helpers;
using Filedash.IntegrationTests.Settings;

namespace Filedash.IntegrationTests;

public class FileOperationTests : IClassFixture<FiledashWebApplicationFactory<Program>>
{
    private readonly FiledashWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonConfig = new() {PropertyNameCaseInsensitive = true};

    public FileOperationTests(FiledashWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task Get_WithValidData_ReturnsCollectionOfItemDetails()
    {
        var expectedFiles = FileHelper
            .GetUploadedFiles()
            .ToArray();

        await InitializeDbAsync(expectedFiles);

        var response = await _client.GetAsync("api/Files/list");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<Result<UploadedFileDetails[]>>(content, _jsonConfig);
        var actualFiles = result.Data;

        Assert.Equal(expectedFiles.Length, actualFiles.Length);
        Assert.True(actualFiles.All(af => expectedFiles.FirstOrDefault(ef => ef.Id == af.Id) != null));
    }

    [Fact]
    public async Task Delete_WithValidId_ReturnsOk()
    {
        var file = FileHelper.GetUploadedFile();

        await InitializeDbAsync(file);

        var response = await _client.DeleteAsync($"api/Files/{file.Id}");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<Result>(content, _jsonConfig);

        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ReturnsBadRequest()
    {
        var file = FileHelper.GetUploadedFile();

        await InitializeDbAsync(file);

        var response = await _client.DeleteAsync($"api/Files/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<Result>(content, _jsonConfig);

        Assert.False(result.IsSuccessful);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task Upload_WithValidFile_ReturnsUploadFileDetails()
    {
        await InitializeDbAsync();

        var (fullFileName, fileBytes) = await FileHelper.GetSmallFileAsync();

        var form = new MultipartFormDataContent();

        form.Add(new ByteArrayContent(fileBytes, 0, fileBytes.Length), "file", fullFileName);

        var response = await _client.PostAsync("api/files/upload", form);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IEnumerable<Result<UploadedFileDetails>>>(content, _jsonConfig);
        
        Assert.NotEmpty(result);
        Assert.Equal(fullFileName, result.First().Data.FullFileName);
    }

    [Fact] public async Task Upload_WithMultipleValidFiles_ReturnsUploadFileDetails()
    {
        await InitializeDbAsync();

        var (fileNameFirst, fileBytesFirst) = await FileHelper.GetSmallFileAsync();
        var (fileNameSecond, fileBytesSecond) = await FileHelper.GetLargeJpgFileAsync();

        var form = new MultipartFormDataContent();

        form.Add(new ByteArrayContent(fileBytesFirst, 0, fileBytesFirst.Length), "file", fileNameFirst);
        form.Add(new ByteArrayContent(fileBytesSecond, 0, fileBytesSecond.Length), "file", fileNameSecond);

        var response = await _client.PostAsync("api/files/upload", form);

        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IEnumerable<Result<UploadedFileDetails>>>(content, _jsonConfig);
        
        Assert.Equal(2, result.Count());
        Assert.True(result.All(f => f.IsSuccessful));
    }

    [Fact]
    public async Task Upload_WithInvalidDuplicateNames_ReturnsBadRequest()
    {
        var file = FileHelper.GetUploadedFile();
        
        await InitializeDbAsync(file);

        var (_, fileBytes) = await FileHelper.GetSmallFileAsync();
        var fullFileName = $"{file.Name}{file.Extension}";
        
        var form = new MultipartFormDataContent();

        form.Add(new ByteArrayContent(fileBytes, 0, fileBytes.Length), "file", fullFileName);

        var response = await _client.PostAsync("api/files/upload", form);

        Assert.False(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IEnumerable<Result<UploadedFileDetails>>>(content, _jsonConfig);

        Assert.Equal(
            $"A file with name '{file.Name}' and extension '{file.Extension}' already exists!",
            result.First().Message);
    }

    [Fact]
    public async Task Upload_WithEmptyFile_ReturnsBadRequest()
    {
        await InitializeDbAsync();

        var (fileName, fileBytes) = await FileHelper.GetEmptyFileAsync();

        var form = new MultipartFormDataContent();

        form.Add(new ByteArrayContent(fileBytes, 0, fileBytes.Length), "file", fileName);

        var response = await _client.PostAsync("api/files/upload", form);

        Assert.False(response.IsSuccessStatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IEnumerable<Result<UploadedFileDetails>>>(content, _jsonConfig);
        
        Assert.Equal(
            "File section and/or file stream  is null or empty!", 
            result.First().Message);
    }
    
    [Fact]
    public async Task Upload_WithEmptyAndLargeFile_ReturnsFailForOneAndSuccesForOther()
    {
        await InitializeDbAsync();

        var (emptyFileName, emptyFileBytes) = await FileHelper.GetEmptyFileAsync();
        var (largeFileName, largeFileBytes) = await FileHelper.GetLargeMp3FileAsync();

        var form = new MultipartFormDataContent();

        form.Add(new ByteArrayContent(emptyFileBytes, 0, emptyFileBytes.Length), "file_1", emptyFileName);
        form.Add(new ByteArrayContent(largeFileBytes, 0, largeFileBytes.Length), "file_2", largeFileName);

        var response = await _client.PostAsync("api/files/upload", form);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<IEnumerable<Result<UploadedFileDetails>>>(content, _jsonConfig);

        var failedResult = result.First(r => !r.IsSuccessful);
        var successResult = result.First(r => r.IsSuccessful);
        
        Assert.Equal(
            "File section and/or file stream  is null or empty!", 
            failedResult.Message);
        
        Assert.Equal(
            largeFileName, successResult.Data.FullFileName);
    }

    private async Task InitializeDbAsync(params UploadedFile[] expectedFiles)
    {
        using var scope = _factory.Services.CreateScope();

        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<FiledashDbContext>();

        await db.Database.EnsureCreatedAsync();

        await DatabaseHelper.CleanDbAsync(db);

        if (expectedFiles != null && expectedFiles.Any())
        {
            await DatabaseHelper.InsertAsync(db, expectedFiles);
        }
    }
}