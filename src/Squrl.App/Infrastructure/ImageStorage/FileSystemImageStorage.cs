namespace Squrl.App.Infrastructure.ImageStorage;

public sealed class FileSystemImageStorage : IImageStorage
{
    private readonly string _imageDirectory;

    public FileSystemImageStorage(IWebHostEnvironment environment)
    {
        const string configuredPath = @"C:\Users\Work\Downloads\test";
        
        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            throw new InvalidOperationException("Image storage root path is not configured.");
        }

        _imageDirectory = Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(environment.ContentRootPath, configuredPath);

        Directory.CreateDirectory(_imageDirectory);
    }

    public async Task SaveAsync(
        string fileName,
        Stream imageStream,
        CancellationToken cancellationToken = default)
    {
        string path = GetSafePath(fileName);

        await using FileStream output = new FileStream(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 64 * 1024,
            useAsync: true);

        await imageStream.CopyToAsync(output, cancellationToken);
    }

    public Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
    {
        string path = GetSafePath(fileName);

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }

    public string GetPhysicalPath(string fileName)
    {
        return GetSafePath(fileName);
    }

    private string GetSafePath(string fileName)
    {
        // Never allow callers to provide a path.
        string safeFileName = Path.GetFileName(fileName);

        if (string.IsNullOrWhiteSpace(safeFileName) || !string.Equals(
                safeFileName,
                fileName,
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "Invalid image filename.",
                nameof(fileName));
        }

        return Path.Combine(_imageDirectory, safeFileName);
    }
}