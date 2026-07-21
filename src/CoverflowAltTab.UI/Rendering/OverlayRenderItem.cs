namespace CoverflowAltTab.UI.Rendering;

public sealed class OverlayRenderItem
{
    public required string Title { get; init; }

    public required string Subtitle { get; init; }

    public bool IsSelected { get; set; }
}
