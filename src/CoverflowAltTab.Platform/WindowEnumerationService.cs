using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Platform;

public sealed class WindowEnumerationService : IWindowEnumerationService
{
    private readonly IWindowInspector _windowInspector;

    public WindowEnumerationService(IWindowInspector windowInspector)
    {
        _windowInspector = windowInspector;
    }

    public IReadOnlyList<WindowInfo> EnumerateWindows()
    {
        var windows = new List<WindowInfo>();

        Win32.EnumWindows((handle, _) =>
        {
            var info = _windowInspector.TryCreateWindowInfo(handle);
            if (info is not null)
            {
                windows.Add(info);
            }

            return true;
        }, IntPtr.Zero);

        return windows;
    }
}
