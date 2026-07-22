using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Platform;

public sealed class DwmWindowThumbnailService : IWindowThumbnailService
{
    private readonly Dictionary<nint, ThumbnailEntry> _entries = new();
    private nint _destinationWindowHandle;

    public bool TryShowThumbnail(nint destinationWindowHandle, nint sourceWindowHandle, WindowBounds destinationBounds, double opacity = 1d)
    {
        if (destinationWindowHandle == 0 || sourceWindowHandle == 0)
        {
            return false;
        }

        if (_destinationWindowHandle != 0 && _destinationWindowHandle != destinationWindowHandle)
        {
            Clear();
        }

        _destinationWindowHandle = destinationWindowHandle;

        var isNewEntry = false;
        if (!_entries.TryGetValue(sourceWindowHandle, out var entry))
        {
            if (Win32.DwmRegisterThumbnail(destinationWindowHandle, sourceWindowHandle, out var thumbnailHandle) != 0 || thumbnailHandle == 0)
            {
                return false;
            }

            entry = new ThumbnailEntry(thumbnailHandle);
            _entries[sourceWindowHandle] = entry;
            isNewEntry = true;
        }

        if (!UpdateEntry(entry, destinationBounds, opacity))
        {
            return false;
        }

        if (isNewEntry && Win32.DwmQueryThumbnailSourceSize(entry.ThumbnailHandle, out var sourceSize) == 0)
        {
            entry.SourceSize = sourceSize;
            return UpdateEntry(entry, destinationBounds, opacity);
        }

        return true;
    }

    public bool BringToFront(nint sourceWindowHandle, WindowBounds destinationBounds, double opacity = 1d)
    {
        if (_destinationWindowHandle == 0 || sourceWindowHandle == 0)
        {
            return false;
        }

        HideThumbnail(sourceWindowHandle);
        return TryShowThumbnail(_destinationWindowHandle, sourceWindowHandle, destinationBounds, opacity);
    }

    public void HideThumbnail(nint sourceWindowHandle)
    {
        if (_entries.Remove(sourceWindowHandle, out var entry))
        {
            Win32.DwmUnregisterThumbnail(entry.ThumbnailHandle);
        }
    }

    public void RetainOnly(IReadOnlyCollection<nint> sourceWindowHandles)
    {
        if (_entries.Count == 0)
        {
            return;
        }

        var staleHandles = _entries.Keys.Where(handle => !sourceWindowHandles.Contains(handle)).ToList();
        foreach (var handle in staleHandles)
        {
            HideThumbnail(handle);
        }
    }

    public void Clear()
    {
        foreach (var entry in _entries.Values)
        {
            Win32.DwmUnregisterThumbnail(entry.ThumbnailHandle);
        }

        _entries.Clear();
        _destinationWindowHandle = 0;
    }

    public void Dispose()
    {
        Clear();
    }

    private static bool UpdateEntry(ThumbnailEntry entry, WindowBounds destinationBounds, double opacity)
    {
        var properties = new Win32.DWM_THUMBNAIL_PROPERTIES
        {
            dwFlags = Win32.DWM_TNP_VISIBLE | Win32.DWM_TNP_RECTDESTINATION | Win32.DWM_TNP_OPACITY | Win32.DWM_TNP_SOURCECLIENTAREAONLY,
            opacity = (byte)Math.Clamp(opacity * 255d, 0d, 255d),
            fVisible = true,
            fSourceClientAreaOnly = true,
            rcDestination = CalculateDestinationRect(entry.SourceSize, destinationBounds),
        };

        return Win32.DwmUpdateThumbnailProperties(entry.ThumbnailHandle, ref properties) == 0;
    }

    private static Win32.RECT CalculateDestinationRect(Win32.SIZE sourceSize, WindowBounds destinationBounds)
    {
        if (sourceSize.cx <= 0 || sourceSize.cy <= 0)
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
            targetWidth / (double)sourceSize.cx,
            targetHeight / (double)sourceSize.cy);

        var scaledWidth = Math.Max(1, (int)Math.Round(sourceSize.cx * scale));
        var scaledHeight = Math.Max(1, (int)Math.Round(sourceSize.cy * scale));
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

    private sealed class ThumbnailEntry(nint thumbnailHandle)
    {
        public nint ThumbnailHandle { get; } = thumbnailHandle;

        public Win32.SIZE SourceSize { get; set; }
    }
}
