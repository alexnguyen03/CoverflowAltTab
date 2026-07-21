using CoverflowAltTab.Core.Abstractions;
using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Core.Sorting;

public sealed class DefaultStableSortStrategy : IWindowSortRule
{
    public string Id => "builtin.default-stable";

    public IReadOnlyList<WindowInfo> Sort(IReadOnlyList<WindowInfo> windows, nint originalForegroundWindowHandle)
    {
        return windows
            .OrderByDescending(window => window.Handle == originalForegroundWindowHandle)
            .ThenBy(window => window.IsMinimized)
            .ThenBy(window => window.Title, StringComparer.OrdinalIgnoreCase)
            .ThenBy(window => window.Handle)
            .ToArray();
    }
}
