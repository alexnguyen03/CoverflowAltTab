namespace CoverflowAltTab.Platform;

public interface IWindowActivationService
{
    bool TryActivateWindow(nint handle);
}
