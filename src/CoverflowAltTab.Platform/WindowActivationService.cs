namespace CoverflowAltTab.Platform;

public sealed class WindowActivationService : IWindowActivationService
{
    public bool TryActivateWindow(nint handle)
    {
        if (handle == 0)
        {
            return false;
        }

        if (Win32.IsIconic(handle))
        {
            Win32.ShowWindowAsync(handle, Win32.SW_RESTORE);
        }

        return Win32.SetForegroundWindow(handle);
    }
}
