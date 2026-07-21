using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Core.Abstractions;

public interface IWindowSortRule
{
    string Id { get; }

    IReadOnlyList<WindowInfo> Sort(IReadOnlyList<WindowInfo> windows, nint originalForegroundWindowHandle);
}
