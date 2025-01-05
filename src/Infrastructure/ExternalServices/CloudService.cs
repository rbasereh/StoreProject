using Amazon.S3;
using Amazon.S3.Model;
using TP.Application.Common.Interfaces;
using TP.Domain.Common;
public class CloudService : ICloudService
{
    private static IAmazonS3 _s3Client;
    private static string urls;
    public CloudService(AppSettings appSettings)
    {
        string baseUrl = $"https://{appSettings.MinioConfig.BaseUrl}";
        urls = baseUrl;
        string accessKey = appSettings.MinioConfig.AccessKey;
        string secretKey = appSettings.MinioConfig.SecretKey;
        var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
        var config = new AmazonS3Config { ForcePathStyle = true, ServiceURL = baseUrl };
        _s3Client = new AmazonS3Client(awsCredentials, config);
    }

    public Task<string> GetObjectUrlAsync(string bucketName, string icon, TimeSpan timeSpan)
    {
        return Task.FromResult($"{urls}/{bucketName}/{icon}");
    }

    public async Task<string> UploadObjectFromFileAsync(string bucketName, string objectName, MemoryStream file, string contentType, string CannedACL = "Public")
    {
        try
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectName,
                InputStream = file,
                ContentType = contentType,
                CannedACL = CannedACL == "Public" ? S3CannedACL.PublicRead : S3CannedACL.Private,

            };

            putRequest.Metadata.Add("x-amz-meta-title", "someTitle");

            PutObjectResponse response = await _s3Client.PutObjectAsync(putRequest);
            return $"{objectName}";

        }
        catch (AmazonS3Exception e)
        {
            throw;
        }
    }
}
