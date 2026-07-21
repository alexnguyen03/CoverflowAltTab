using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Core.Services;

public sealed record WindowPipelineResult(
    IReadOnlyList<WindowInfo> Windows,
    int RawCount,
    int FilteredCount,
    string SortStrategyId);
