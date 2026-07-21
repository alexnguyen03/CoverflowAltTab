namespace CoverflowAltTab.Platform;

public sealed class WindowActivationService : IWindowActivationService
{
    public bool TryActivateWindow(nint handle)
    {
        if (handle == 0 || !Win32.IsWindow(handle))
        {
            return false;
        }

        if (Win32.GetForegroundWindow() == handle)
        {
            return true;
        }

        if (Win32.IsIconic(handle))
        {
            Win32.ShowWindowAsync(handle, Win32.SW_RESTORE);
        }
        else
        {
            Win32.ShowWindowAsync(handle, Win32.SW_SHOW);
        }

        if (Win32.SetForegroundWindow(handle))
        {
            return true;
        }

        return TryActivateWithThreadInput(handle);
    }

    private static bool TryActivateWithThreadInput(nint handle)
    {
        var foregroundWindow = Win32.GetForegroundWindow();
        var currentThreadId = Win32.GetCurrentThreadId();
        var foregroundThreadId = foregroundWindow == 0
            ? currentThreadId
            : Win32.GetWindowThreadProcessId(foregroundWindow, out _);
        var targetThreadId = Win32.GetWindowThreadProcessId(handle, out _);
        var attachedToForeground = false;
        var attachedToTarget = false;

        try
        {
            if (foregroundThreadId != currentThreadId)
            {
                attachedToForeground = Win32.AttachThreadInput(currentThreadId, foregroundThreadId, true);
            }

            if (targetThreadId != currentThreadId)
            {
                attachedToTarget = Win32.AttachThreadInput(currentThreadId, targetThreadId, true);
            }

            Win32.BringWindowToTop(handle);
            Win32.SetActiveWindow(handle);
            Win32.SetFocus(handle);

            return Win32.SetForegroundWindow(handle) || Win32.GetForegroundWindow() == handle;
        }
        finally
        {
            if (attachedToTarget)
            {
                Win32.AttachThreadInput(currentThreadId, targetThreadId, false);
            }

            if (attachedToForeground)
            {
                Win32.AttachThreadInput(currentThreadId, foregroundThreadId, false);
            }
        }
    }
}
