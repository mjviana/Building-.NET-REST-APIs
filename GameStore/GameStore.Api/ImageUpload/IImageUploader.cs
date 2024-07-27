namespace GameStore.Api.ImageUpload;

public interface IImageUploader
{
    Task<string> UploadImageAsync(IFormFile file);
}
