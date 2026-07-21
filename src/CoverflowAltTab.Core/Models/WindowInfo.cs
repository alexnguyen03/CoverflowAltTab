namespace CoverflowAltTab.Core.Models;

public sealed record WindowInfo(
    nint Handle,
    int ProcessId,
    string ProcessName,
    string Title,
    string ClassName,
    bool IsVisible,
    bool IsMinimized,
    bool IsToolWindow,
    bool IsCloaked,
    bool IsShellWindow,
    bool HasOwnerWindow,
    WindowBounds? Bounds);
