using CoverflowAltTab.Core.Abstractions;
using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Core.Services;

public sealed class WindowPipeline
{
    public WindowPipelineResult ProcessWindows(
        IReadOnlyList<WindowInfo> rawWindows,
        IReadOnlyList<IWindowFilterRule> filters,
        IWindowSortRule sortRule,
        nint originalForegroundWindowHandle)
    {
        IEnumerable<WindowInfo> query = rawWindows;

        foreach (var filter in filters)
        {
            query = query.Where(filter.ShouldInclude);
        }

        var filtered = query.ToArray();
        var sorted = sortRule.Sort(filtered, originalForegroundWindowHandle);

        return new WindowPipelineResult(sorted, rawWindows.Count, filtered.Length, sortRule.Id);
    }
}
