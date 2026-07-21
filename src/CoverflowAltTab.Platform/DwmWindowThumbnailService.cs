using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Platform;

public sealed class DwmWindowThumbnailService : IWindowThumbnailService
{
    private nint _thumbnailHandle;
    private nint _destinationWindowHandle;
    private nint _sourceWindowHandle;

    public bool TryShowThumbnail(nint destinationWindowHandle, nint sourceWindowHandle, WindowBounds destinationBounds)
    {
        if (destinationWindowHandle == 0 || sourceWindowHandle == 0)
        {
            return false;
        }

        if (_thumbnailHandle != 0 &&
            _destinationWindowHandle == destinationWindowHandle &&
            _sourceWindowHandle == sourceWindowHandle)
        {
            return TryUpdateBounds(destinationBounds);
        }

        Clear();

        if (Win32.DwmRegisterThumbnail(destinationWindowHandle, sourceWindowHandle, out _thumbnailHandle) != 0 || _thumbnailHandle == 0)
        {
            _thumbnailHandle = 0;
            return false;
        }

        _destinationWindowHandle = destinationWindowHandle;
        _sourceWindowHandle = sourceWindowHandle;

        return TryUpdateBounds(destinationBounds);
    }

    public bool TryUpdateBounds(WindowBounds destinationBounds)
    {
        if (_thumbnailHandle == 0)
        {
            return false;
        }

        var properties = new Win32.DWM_THUMBNAIL_PROPERTIES
        {
            dwFlags = Win32.DWM_TNP_VISIBLE | Win32.DWM_TNP_RECTDESTINATION | Win32.DWM_TNP_OPACITY,
            opacity = 255,
            fVisible = true,
            rcDestination = new Win32.RECT
            {
                Left = destinationBounds.Left,
                Top = destinationBounds.Top,
                Right = destinationBounds.Right,
                Bottom = destinationBounds.Bottom,
            },
        };

        return Win32.DwmUpdateThumbnailProperties(_thumbnailHandle, ref properties) == 0;
    }

    public void Clear()
    {
        if (_thumbnailHandle != 0)
        {
            Win32.DwmUnregisterThumbnail(_thumbnailHandle);
            _thumbnailHandle = 0;
        }

        _destinationWindowHandle = 0;
        _sourceWindowHandle = 0;
    }

    public void Dispose()
    {
        Clear();
    }
}
