using CoverflowAltTab.Core.Abstractions;
using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Core.Filters;

public sealed class BuiltInWindowFilter : IWindowFilterRule
{
    private static readonly HashSet<string> ExcludedClassNames = new(StringComparer.Ordinal)
    {
        "Progman",
        "Shell_TrayWnd",
        "DV2ControlHost",
        "Windows.UI.Core.CoreWindow",
        "OperationStatusWindow",
    };

    private readonly int _hostProcessId;

    public BuiltInWindowFilter(int hostProcessId)
    {
        _hostProcessId = hostProcessId;
    }

    public string Name => "builtin.default-filter";

    public bool ShouldInclude(WindowInfo window)
    {
        return GetExclusionReason(window) is null;
    }

    public string? GetExclusionReason(WindowInfo window)
    {
        if (window.ProcessId == _hostProcessId)
        {
            return "host-process";
        }

        if (!window.IsVisible)
        {
            return "invisible";
        }

        if (string.IsNullOrWhiteSpace(window.Title))
        {
            return "empty-title";
        }

        if (window.IsToolWindow)
        {
            return "tool-window";
        }

        if (window.IsCloaked)
        {
            return "cloaked";
        }

        if (window.IsShellWindow)
        {
            return "shell-window";
        }

        if (window.HasOwnerWindow)
        {
            return "owned-window";
        }

        if (ExcludedClassNames.Contains(window.ClassName))
        {
            return $"class:{window.ClassName}";
        }

        return null;
    }
}
