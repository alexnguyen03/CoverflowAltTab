using CoverflowAltTab.Core.Abstractions;
using CoverflowAltTab.Core.Models;
using CoverflowAltTab.Extensibility;

namespace CoverflowAltTab.Host.Adapters;

public sealed class ExtensionSortStrategyAdapter : IWindowSortRule
{
    private readonly ISortStrategy _strategy;

    public ExtensionSortStrategyAdapter(ISortStrategy strategy)
    {
        _strategy = strategy;
    }

    public string Id => _strategy.Id;

    public IReadOnlyList<WindowInfo> Sort(IReadOnlyList<WindowInfo> windows, nint originalForegroundWindowHandle)
    {
        return _strategy.Sort(windows);
    }
}
