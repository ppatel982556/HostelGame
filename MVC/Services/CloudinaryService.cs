using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using MVC.Services.Interfaces;

namespace MVC.Services;

public sealed class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration configuration)
    {
        var cloudName = configuration["Cloudinary:CloudName"];
        var apiKey = configuration["Cloudinary:ApiKey"];
        var apiSecret = configuration["Cloudinary:ApiSecret"];

        if (string.IsNullOrWhiteSpace(cloudName) ||
            string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new InvalidOperationException(
                "Cloudinary configuration is missing.");
        }

        var account = new Account(
            cloudName,
            apiKey,
            apiSecret);

        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<(string PublicId, string SecureUrl)>
        UploadImageAsync(
            Stream imageStream,
            string fileName,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(imageStream);

        if (!imageStream.CanRead)
        {
            throw new ArgumentException(
                "The image stream must be readable.",
                nameof(imageStream));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException(
                "A file name is required.",
                nameof(fileName));
        }

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, imageStream),
            Folder = "hostel-game/scenes",
            UseFilename = false,
            UniqueFilename = true
        };

        var result = await _cloudinary.UploadAsync(
            uploadParams,
            cancellationToken);

        if (result.Error is not null ||
            string.IsNullOrWhiteSpace(result.PublicId) ||
            result.SecureUrl is null)
        {
            throw new InvalidOperationException(
                $"Cloudinary image upload failed: " +
                $"{result.Error?.Message ?? "No image URL was returned."}");
        }

        return (result.PublicId, result.SecureUrl.ToString());
    }

    public async Task DeleteImageAsync(string publicId)
    {
        if (string.IsNullOrWhiteSpace(publicId))
        {
            throw new ArgumentException(
            "A Cloudinary public ID is required.",
            nameof(publicId));
        }


        var deleteParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image
        };

        var result = await _cloudinary.DestroyAsync(deleteParams);

        if (result.Error is not null)
        {
            throw new InvalidOperationException(
                $"Cloudinary image deletion failed: {result.Error.Message}");
        }

        if (result.Result != "ok" && result.Result != "not found")
        {
            throw new InvalidOperationException(
                $"Cloudinary image deletion returned: {result.Result}");
        }


    }

}