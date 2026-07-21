namespace CoverflowAltTab.Extensibility;

internal sealed class HostContext : IHostContext
{
    private readonly ExtensionRegistry _registry;
    private readonly ExtensionRegistrationSummary _summary;

    public HostContext(ExtensionRegistry registry, ILogger logger, ExtensionRegistrationSummary summary)
    {
        _registry = registry;
        Logger = logger;
        _summary = summary;
    }

    public ILogger Logger { get; }

    public void RegisterWindowFilter(IWindowFilter filter)
    {
        _registry.AddWindowFilter(filter);
        _summary.IncrementWindowFilters();
    }

    public void RegisterSortStrategy(ISortStrategy strategy)
    {
        _registry.AddSortStrategy(strategy);
        _summary.IncrementSortStrategies();
    }

    public void RegisterOverlayDismissBehavior(IOverlayDismissBehavior behavior)
    {
        _registry.AddOverlayDismissBehavior(behavior);
        _summary.IncrementOverlayDismissBehaviors();
    }
}
