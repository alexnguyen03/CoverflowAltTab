using CoverflowAltTab.Core.Abstractions;
using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Core.Filters;

public sealed class BuiltInWindowFilter : IWindowFilterRule
{
    private readonly int _hostProcessId;

    public BuiltInWindowFilter(int hostProcessId)
    {
        _hostProcessId = hostProcessId;
    }

    public string Name => "builtin.default-filter";

    public bool ShouldInclude(WindowInfo window)
    {
        if (window.ProcessId == _hostProcessId)
        {
            return false;
        }

        if (!window.IsVisible)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(window.Title))
        {
            return false;
        }

        if (window.IsToolWindow || window.IsCloaked || window.IsShellWindow)
        {
            return false;
        }

        return true;
    }
}
