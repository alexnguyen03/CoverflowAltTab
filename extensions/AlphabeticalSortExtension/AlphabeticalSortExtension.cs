using CoverflowAltTab.Core.Models;
using CoverflowAltTab.Extensibility;

namespace AlphabeticalSortExtension;

public sealed class AlphabeticalSortExtension : IExtension
{
    public string Id => "sample.sort-alphabetically";

    public string Name => "Alphabetical Sort";

    public void Initialize(IHostContext context)
    {
        context.RegisterSortStrategy(new AlphabeticalTitleSortStrategy());
        context.Logger.Info("AlphabeticalSortExtension registered AlphabeticalTitleSortStrategy.");
    }
}

public sealed class AlphabeticalTitleSortStrategy : ISortStrategy
{
    public string Id => "sample.sort-alphabetically";

    public IReadOnlyList<WindowInfo> Sort(IReadOnlyList<WindowInfo> windows)
    {
        return windows
            .OrderBy(window => window.Title, StringComparer.OrdinalIgnoreCase)
            .ThenBy(window => window.ProcessName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(window => window.Handle)
            .ToArray();
    }
}
