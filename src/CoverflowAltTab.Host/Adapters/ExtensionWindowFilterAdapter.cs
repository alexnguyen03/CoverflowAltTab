using CoverflowAltTab.Core.Abstractions;
using CoverflowAltTab.Core.Models;
using CoverflowAltTab.Extensibility;

namespace CoverflowAltTab.Host.Adapters;

public sealed class ExtensionWindowFilterAdapter : IWindowFilterRule
{
    private readonly IWindowFilter _filter;

    public ExtensionWindowFilterAdapter(IWindowFilter filter)
    {
        _filter = filter;
    }

    public string Name => _filter.GetType().FullName ?? _filter.GetType().Name;

    public bool ShouldInclude(WindowInfo window)
    {
        return _filter.ShouldInclude(window);
    }
}
