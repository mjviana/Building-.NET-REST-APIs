using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace GameStore.Api.ImageUpload;

public class ImageUploader : IImageUploader
{
    private readonly BlobContainerClient _blobContainerClient;

    public ImageUploader(BlobContainerClient blobContainerClient)
    {
        _blobContainerClient = blobContainerClient;
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        await _blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        var blobClient = _blobContainerClient.GetBlobClient(file.FileName);
        await blobClient.DeleteIfExistsAsync();

        using var fileStream = file.OpenReadStream();
        await blobClient.UploadAsync(
            fileStream,
            new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });

        return blobClient.Uri.ToString();
    }
}
