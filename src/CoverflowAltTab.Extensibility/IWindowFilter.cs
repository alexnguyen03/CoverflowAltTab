using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.Extensibility;

public interface IWindowFilter
{
    bool ShouldInclude(WindowInfo window);
}
