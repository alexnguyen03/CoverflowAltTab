using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Core.Abstractions;

public interface IWindowFilterRule
{
    string Name { get; }

    bool ShouldInclude(WindowInfo window);
}
