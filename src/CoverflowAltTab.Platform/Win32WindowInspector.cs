using System.Diagnostics;
using System.Text;
using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Platform;

public sealed class Win32WindowInspector : IWindowInspector
{
    public WindowInfo? TryCreateWindowInfo(nint handle)
    {
        if (handle == 0)
        {
            return null;
        }

        var isVisible = Win32.IsWindowVisible(handle);
        var isMinimized = Win32.IsIconic(handle);
        var isShellWindow = handle == Win32.GetShellWindow();
        var isToolWindow = (Win32.GetWindowLongPtr(handle, Win32.GWL_EXSTYLE).ToInt64() & Win32.WS_EX_TOOLWINDOW) != 0;
        var isCloaked = TryGetCloaked(handle);
        var title = GetWindowTitle(handle);
        var className = GetClassName(handle);
        var processId = GetProcessId(handle);
        var processName = NormalizeProcessName(GetProcessName(processId), className);
        var bounds = TryGetBounds(handle);
        var hasOwnerWindow = Win32.GetWindow(handle, Win32.GW_OWNER) != 0;

        return new WindowInfo(
            handle,
            processId,
            processName,
            title,
            className,
            isVisible,
            isMinimized,
            isToolWindow,
            isCloaked,
            isShellWindow,
            hasOwnerWindow,
            bounds);
    }

    private static string GetWindowTitle(nint handle)
    {
        var length = Win32.GetWindowTextLength(handle);
        if (length <= 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder(length + 1);
        Win32.GetWindowText(handle, builder, builder.Capacity);
        return builder.ToString().Trim();
    }

    private static int GetProcessId(nint handle)
    {
        Win32.GetWindowThreadProcessId(handle, out var processId);
        return unchecked((int)processId);
    }

    private static string GetProcessName(int processId)
    {
        if (processId <= 0)
        {
            return string.Empty;
        }

        try
        {
            return Process.GetProcessById(processId).ProcessName;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string GetClassName(nint handle)
    {
        var builder = new StringBuilder(256);
        var length = Win32.GetClassName(handle, builder, builder.Capacity);
        return length <= 0 ? string.Empty : builder.ToString();
    }

    private static string NormalizeProcessName(string processName, string className)
    {
        if (!string.IsNullOrWhiteSpace(processName))
        {
            return processName;
        }

        if (!string.IsNullOrWhiteSpace(className))
        {
            return className;
        }

        return "Unknown";
    }

    private static bool TryGetCloaked(nint handle)
    {
        try
        {
            return Win32.DwmGetWindowAttribute(handle, Win32.DWMWA_CLOAKED, out int cloaked, sizeof(int)) == 0 && cloaked != 0;
        }
        catch
        {
            return false;
        }
    }

    private static WindowBounds? TryGetBounds(nint handle)
    {
        return Win32.GetWindowRect(handle, out var rect)
            ? new WindowBounds(rect.Left, rect.Top, rect.Right, rect.Bottom)
            : null;
    }
}
