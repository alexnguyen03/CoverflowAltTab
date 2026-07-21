using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Platform;

public interface IWindowEnumerationService
{
    IReadOnlyList<WindowInfo> EnumerateWindows();
}
