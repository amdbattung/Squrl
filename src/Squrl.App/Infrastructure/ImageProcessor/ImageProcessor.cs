using SkiaSharp;
using Squrl.App.Infrastructure.ImageStorage;

namespace Squrl.App.Infrastructure.ImageProcessor;

public class ImageProcessor : IImageProcessor
{
    private readonly IImageStorage _storage;
    private readonly int _maxSourceDimension;
    private readonly int _maxDimension;
    private readonly int _quality;

    public ImageProcessor(IImageStorage storage)
    {
        _storage = storage;
        _maxSourceDimension = 16000;
        _maxDimension = 800;
        _quality = 80;
    }

    public async Task<string> SaveAsync(
        Stream input,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
            
        using SKManagedStream inputStream = new SKManagedStream(input);

        using SKBitmap? original = SKBitmap.Decode(inputStream);

        if (original is null)
        {
            throw new InvalidDataException("Invalid image.");
        }
            
        if (original.Width <= 0 || original.Height <= 0)
        {
            throw new InvalidDataException("Image has invalid dimensions.");
        }
        
        if (original.Width > _maxSourceDimension ||
            original.Height > _maxSourceDimension)
        {
            throw new InvalidDataException("Image dimensions are too large.");
        }

        // The image is stretched/squished into a square.
        using SKBitmap resizedOriginal = ResizeToSquare(original, _maxDimension);

        using SKImage? image = SKImage.FromBitmap(resizedOriginal);

        using SKData? encodedImage = image.Encode(SKEncodedImageFormat.Webp, _quality);
            
        if (encodedImage is null)
        {
            throw new InvalidOperationException("Failed to encode image.");
        }

        string fileName = $"{Guid.NewGuid():N}.webp";

        await using Stream? output = encodedImage.AsStream();

        await _storage.SaveAsync(fileName, output, cancellationToken);

        return fileName;
    }

    private static SKBitmap ResizeToSquare(SKBitmap source, int size)
    {
        SKBitmap result = new SKBitmap(
            size,
            size,
            SKColorType.Rgba8888,
            SKAlphaType.Premul);

        using SKCanvas canvas = new SKCanvas(result);

        canvas.Clear(SKColors.White);

        using SKPaint paint = new SKPaint();
        paint.IsAntialias = true;
        
        SKSamplingOptions sampling = new SKSamplingOptions(
            SKFilterMode.Linear,
            SKMipmapMode.Linear);

        SKRect destination = new SKRect(
            0,
            0,
            size,
            size);

        canvas.DrawBitmap(
            source,
            destination,
            sampling,
            paint);

        canvas.Flush();

        return result;
    }
}