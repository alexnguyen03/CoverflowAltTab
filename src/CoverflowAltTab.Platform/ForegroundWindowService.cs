namespace CoverflowAltTab.Platform;

public sealed class ForegroundWindowService : IForegroundWindowService
{
    public nint GetForegroundWindowHandle()
    {
        return Win32.GetForegroundWindow();
    }
}
