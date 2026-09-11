using Squrl.App.Common;

namespace Squrl.App.Infrastructure.ImageProcessor;

public interface IImageProcessor
{
    Task<string> SaveAsync(Stream input, CancellationToken cancellationToken = default);
}