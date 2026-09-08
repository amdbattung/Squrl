namespace Squrl.App.Infrastructure.ImageStorage;

public interface IImageStorage
{
    Task SaveAsync(string fileName, Stream imageStream, CancellationToken cancellationToken = default);

    Task DeleteAsync(string fileName, CancellationToken cancellationToken = default);

    string GetPhysicalPath(string fileName);
}