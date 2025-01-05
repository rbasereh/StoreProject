using TP.Application.Common.Interfaces;

namespace TP.Api.Services;


public class FileUploader(ICloudService cloudService)
{
    public async Task<string> UploadAsync(IFormFile file, string bucketName)
    {
        if (file == null || file.Length == 0)
            throw new Exception("File is not selected or empty");

        string fileName = $"{Path.GetFileNameWithoutExtension(Guid.NewGuid().ToString())}_{DateTime.UtcNow.Ticks}{Path.GetExtension(file.FileName)}";
        fileName = fileName.Replace(" ", "");
        string fileUrl;
        using (MemoryStream memoryStream = new())
        {
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            fileUrl = await cloudService.UploadObjectFromFileAsync(bucketName, fileName, memoryStream, file.ContentType);
        }
        return fileUrl;
    }
}
