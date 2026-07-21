using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Core.Services;

public sealed record SessionOpenResult(
    bool Success,
    string Reason,
    SwitchSession? Session,
    WindowPipelineResult PipelineResult);
