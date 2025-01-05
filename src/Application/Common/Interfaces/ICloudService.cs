namespace TP.Application.Common.Interfaces;

public class BucketName
{
    public const string UserProfile = "user-profile";
    public const string UserDocuments = "user-documents";
}
public interface ICloudService
{
    Task<string> UploadObjectFromFileAsync(string bucketName, string objectName, MemoryStream file, string contentType, string CannedACL = "Public");

    Task<string> GetObjectUrlAsync(string bucketName, string icon, TimeSpan timeSpan);

}
