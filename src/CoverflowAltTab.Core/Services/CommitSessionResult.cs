using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Core.Services;

public sealed record CommitSessionResult(
    bool Success,
    string Reason,
    WindowInfo? SelectedWindow,
    Guid? SessionId);
