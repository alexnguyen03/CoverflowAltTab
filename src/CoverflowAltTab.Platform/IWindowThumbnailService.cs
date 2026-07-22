using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Platform;

public interface IWindowThumbnailService : IDisposable
{
    bool TryShowThumbnail(nint destinationWindowHandle, nint sourceWindowHandle, WindowBounds destinationBounds, double opacity = 1d);

    bool BringToFront(nint sourceWindowHandle, WindowBounds destinationBounds, double opacity = 1d);

    void HideThumbnail(nint sourceWindowHandle);

    void RetainOnly(IReadOnlyCollection<nint> sourceWindowHandles);

    void Clear();
}
