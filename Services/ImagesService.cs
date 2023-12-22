using MWF_Web_Api.Models;
using MWF_Web_Api.Utility;

namespace MWF_Web_Api.Services;

public class ImagesService
{
    private readonly ILogger<ImagesService> _logger;
    private readonly string storageFolderPath = "/MWF_Images";

    public ImagesService(ILogger<ImagesService> logger)
    {
        _logger = logger;
        Initialize();
    }

    private void Initialize()
    {
        if (!Directory.Exists(storageFolderPath))
        {
            Directory.CreateDirectory(storageFolderPath);
        }
    }

    internal async Task<Result<bool>> SaveImage(Image image, int id, CancellationToken cancellationToken)
    {
        var fileName = $"{id}.{image.Extension}";

        try
        {
            var path = Path.Combine(storageFolderPath, fileName);

            await using var fs = new FileStream(path, FileMode.OpenOrCreate);
            fs.Position = 0;

            await fs.WriteAsync(image.Bytes, cancellationToken);
            fs.Close();
            return Result<bool>.Success();
        }
        catch (Exception e)
        {
            _logger.LogWarning("Attempt to save image {FileName} led to exception {Exception}. " +
                               "Exception message: {ExceptionMessage}", fileName, e, e.Message);
            return Result<bool>.NotSuccess(e.Message);
        }
    }

    internal async Task<Result<byte[]?>> GetImage(int id, CancellationToken cancellationToken)
    {
        //var path = $"{storageFolderPath}/{id}.jpg";

        try
        {
            if (TryGetFileNameFromStorage(id, out var path))
            {
                var bytes = await File.ReadAllBytesAsync(path!, cancellationToken);
                return Result<byte[]>.Success(bytes)!;
            }

            return Result<byte[]>.NotSuccess(null, "Image not found.")!;
        }
        catch (Exception e)
        {
            _logger.LogWarning("Attempt to download image for Id {Id} led to exception {Exception}. " +
                               "Exception message: {ExceptionMessage}", id, e, e.Message);

            return Result<byte[]>.NotSuccess(null, e.Message);
        }
    }

    internal ImageExtension GetImageFileExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new ArgumentNullException(nameof(extension));
        }

        switch (extension.ToLowerInvariant())
        {
            case ".jpg":
                return ImageExtension.jpg;

            case ".png":
                return ImageExtension.png;

            default:
                throw new NotSupportedException($"Image extension {extension} is not supported.");
        }
    }

    private bool TryGetFileNameFromStorage(int id, out string? fileName)
    {
        fileName = Directory.EnumerateFiles(storageFolderPath).FirstOrDefault(f =>
            Path.GetFileNameWithoutExtension(f).ToLowerInvariant().Equals(id.ToString(), StringComparison.OrdinalIgnoreCase));

        if (fileName is null)
        {
            return false;
        }

        return true;
    }
}