using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.UI.Rendering;

public sealed class CoverflowOverlayRenderer : IOverlayRenderer
{
    private const double StageWidth = 900d;
    private const double StageHeight = 250d;
    private const double CardWidth = 260d;
    private const double CardHeight = 146d;
    private const double CenterX = (StageWidth - CardWidth) / 2d;
    private const double CenterY = 42d;
    private const double HorizontalStep = 142d;
    private const double DepthStep = 18d;
    private const double MaxRotation = 58d;
    private const double MinOpacity = 0.2d;

    public string Id => "builtin.coverflow";

    public OverlayRenderModel BuildModel(SwitchSession session)
    {
        var items = new List<OverlayRenderItem>(session.Windows.Count);

        for (var index = 0; index < session.Windows.Count; index++)
        {
            items.Add(BuildItem(session.Windows[index], index, session.SelectedIndex, session.Windows.Count));
        }

        return new OverlayRenderModel
        {
            RendererId = Id,
            HeaderTitle = "CoverflowAltTab MVP",
            HeaderSubtitle = $"Renderer: Coverflow | Sort: {session.ActiveSortStrategyId}",
            Items = items,
            SelectedIndex = session.SelectedIndex,
            UsesFreeformLayout = true,
            StageWidth = StageWidth,
            StageHeight = StageHeight,
        };
    }

    public void UpdateSelection(OverlayRenderModel model, int selectedIndex)
    {
        model.SelectedIndex = selectedIndex;

        for (var index = 0; index < model.Items.Count; index++)
        {
            ApplySlotLayout(model.Items[index], index - selectedIndex, model.Items.Count);
        }
    }

    private static OverlayRenderItem BuildItem(WindowInfo window, int index, int selectedIndex, int totalCount)
    {
        var item = new OverlayRenderItem
        {
            Title = window.Title,
            Subtitle = string.IsNullOrWhiteSpace(window.ProcessName) ? "Unknown" : window.ProcessName,
            Width = CardWidth,
            Height = CardHeight,
        };

        ApplySlotLayout(item, index - selectedIndex, totalCount);
        return item;
    }

    private static void ApplySlotLayout(OverlayRenderItem item, int offset, int totalCount)
    {
        item.IsSelected = offset == 0;
        item.X = CenterX + (offset * HorizontalStep);
        item.Y = CenterY + (Math.Abs(offset) * DepthStep);
        item.Scale = offset == 0 ? 1.14d : Math.Max(0.58d, 0.92d - (Math.Abs(offset) * 0.11d));
        item.Rotation = offset == 0 ? 0d : Math.Clamp(offset * -20d, -MaxRotation, MaxRotation);
        item.Opacity = offset == 0 ? 1d : Math.Max(MinOpacity, 0.82d - (Math.Abs(offset) * 0.18d));
        item.ZIndex = totalCount - Math.Abs(offset);
    }
}
