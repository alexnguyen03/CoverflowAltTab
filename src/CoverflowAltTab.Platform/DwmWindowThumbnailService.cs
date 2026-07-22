using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Platform;

public sealed class DwmWindowThumbnailService : IWindowThumbnailService
{
    private nint _thumbnailHandle;
    private nint _destinationWindowHandle;
    private nint _sourceWindowHandle;
    private Win32.SIZE _sourceSize;

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
        _sourceSize = default;

        if (Win32.DwmQueryThumbnailSourceSize(_thumbnailHandle, out var sourceSize) == 0)
        {
            _sourceSize = sourceSize;
        }

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
            rcDestination = CalculateDestinationRect(destinationBounds),
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
        _sourceSize = default;
    }

    public void Dispose()
    {
        Clear();
    }

    private Win32.RECT CalculateDestinationRect(WindowBounds destinationBounds)
    {
        if (_sourceSize.cx <= 0 || _sourceSize.cy <= 0)
        {
            return new Win32.RECT
            {
                Left = destinationBounds.Left,
                Top = destinationBounds.Top,
                Right = destinationBounds.Right,
                Bottom = destinationBounds.Bottom,
            };
        }

        var targetWidth = Math.Max(1, destinationBounds.Right - destinationBounds.Left);
        var targetHeight = Math.Max(1, destinationBounds.Bottom - destinationBounds.Top);
        var scale = Math.Min(
            targetWidth / (double)_sourceSize.cx,
            targetHeight / (double)_sourceSize.cy);

        var scaledWidth = Math.Max(1, (int)Math.Round(_sourceSize.cx * scale));
        var scaledHeight = Math.Max(1, (int)Math.Round(_sourceSize.cy * scale));
        var offsetX = destinationBounds.Left + ((targetWidth - scaledWidth) / 2);
        var offsetY = destinationBounds.Top + ((targetHeight - scaledHeight) / 2);

        return new Win32.RECT
        {
            Left = offsetX,
            Top = offsetY,
            Right = offsetX + scaledWidth,
            Bottom = offsetY + scaledHeight,
        };
    }
}
