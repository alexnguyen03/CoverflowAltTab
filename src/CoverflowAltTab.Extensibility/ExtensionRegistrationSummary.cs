namespace CoverflowAltTab.Extensibility;

internal sealed class ExtensionRegistrationSummary
{
    public int RegisteredWindowFilters { get; private set; }

    public int RegisteredSortStrategies { get; private set; }

    public int RegisteredOverlayDismissBehaviors { get; private set; }

    public void IncrementWindowFilters()
    {
        RegisteredWindowFilters++;
    }

    public void IncrementSortStrategies()
    {
        RegisteredSortStrategies++;
    }

    public void IncrementOverlayDismissBehaviors()
    {
        RegisteredOverlayDismissBehaviors++;
    }
}
