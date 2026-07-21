namespace CoverflowAltTab.UI.Rendering;

public sealed class OverlayRenderModel
{
    public required string RendererId { get; init; }

    public required string HeaderTitle { get; init; }

    public required string HeaderSubtitle { get; init; }

    public required IReadOnlyList<OverlayRenderItem> Items { get; init; }

    public int SelectedIndex { get; set; }

    public bool UsesFreeformLayout { get; init; }

    public double StageWidth { get; init; }

    public double StageHeight { get; init; }
}
