namespace CoverflowAltTab.Extensibility;

public interface IHostContext
{
    void RegisterWindowFilter(IWindowFilter filter);

    void RegisterSortStrategy(ISortStrategy strategy);

    void RegisterOverlayDismissBehavior(IOverlayDismissBehavior behavior);

    ILogger Logger { get; }
}
