namespace MVC.Services.Interfaces;

public interface ICloudinaryService
{
    Task<(string PublicId, string SecureUrl)> UploadImageAsync(
        Stream imageStream,
        string fileName,
        CancellationToken cancellationToken = default);

    Task DeleteImageAsync(string publicId);
}