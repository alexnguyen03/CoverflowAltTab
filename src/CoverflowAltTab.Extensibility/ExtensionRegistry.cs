namespace CoverflowAltTab.Extensibility;

public sealed class ExtensionRegistry
{
    private readonly List<IWindowFilter> _windowFilters = [];
    private readonly List<ISortStrategy> _sortStrategies = [];
    private readonly List<IOverlayDismissBehavior> _overlayDismissBehaviors = [];

    public IReadOnlyList<IWindowFilter> WindowFilters => _windowFilters;

    public IReadOnlyList<ISortStrategy> SortStrategies => _sortStrategies;

    public IReadOnlyList<IOverlayDismissBehavior> OverlayDismissBehaviors => _overlayDismissBehaviors;

    internal void AddWindowFilter(IWindowFilter filter)
    {
        _windowFilters.Add(filter);
    }

    internal void AddSortStrategy(ISortStrategy strategy)
    {
        _sortStrategies.Add(strategy);
    }

    internal void AddOverlayDismissBehavior(IOverlayDismissBehavior behavior)
    {
        _overlayDismissBehaviors.Add(behavior);
    }
}
