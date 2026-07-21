using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Platform;

public interface IWindowThumbnailService : IDisposable
{
    bool TryShowThumbnail(nint destinationWindowHandle, nint sourceWindowHandle, WindowBounds destinationBounds);

    bool TryUpdateBounds(WindowBounds destinationBounds);

    void Clear();
}
