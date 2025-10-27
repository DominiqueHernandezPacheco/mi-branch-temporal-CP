namespace Application.Core.Infraestructure.Services;

public interface IFileStorageService
{
    Task<string> ConvertBase64ToFileAsync(string base64Content, string filePath);
    Task<string> ConvertFileToBase64Async(string filePath);
    string GenerateUniqueFilePath(string path);
    bool DeleteFile(string filePath);
}

