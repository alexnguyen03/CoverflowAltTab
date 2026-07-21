using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Extensibility;

public interface ISortStrategy
{
    string Id { get; }

    IReadOnlyList<WindowInfo> Sort(IReadOnlyList<WindowInfo> windows);
}
