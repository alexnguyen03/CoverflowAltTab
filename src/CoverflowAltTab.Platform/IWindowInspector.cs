using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Platform;

public interface IWindowInspector
{
    WindowInfo? TryCreateWindowInfo(nint handle);
}
