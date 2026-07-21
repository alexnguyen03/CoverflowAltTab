namespace CoverflowAltTab.Extensibility;

public sealed record ExtensionLoadResult(
    string Source,
    string ExtensionId,
    string ExtensionName,
    string Version,
    string Description,
    bool Success,
    string Message,
    int RegisteredWindowFilters,
    int RegisteredSortStrategies,
    int RegisteredOverlayDismissBehaviors);
