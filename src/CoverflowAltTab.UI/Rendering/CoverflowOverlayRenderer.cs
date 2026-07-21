using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.UI.Rendering;

public sealed class CoverflowOverlayRenderer : IOverlayRenderer
{
    private const double StageWidth = 820d;
    private const double StageHeight = 220d;
    private const double CardWidth = 240d;
    private const double CardHeight = 132d;
    private const double CenterX = (StageWidth - CardWidth) / 2d;
    private const double CenterY = 34d;
    private const double HorizontalStep = 128d;
    private const double DepthStep = 22d;
    private const double MaxRotation = 55d;
    private const double MinOpacity = 0.32d;

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
            HeaderSubtitle = $"Renderer: Coverflow • Sort: {session.ActiveSortStrategyId}",
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
            var item = model.Items[index];
            var offset = index - selectedIndex;

            item.IsSelected = offset == 0;
            item.X = CenterX + (offset * HorizontalStep);
            item.Y = CenterY + (Math.Abs(offset) * DepthStep);
            item.Scale = offset == 0 ? 1.08d : Math.Max(0.72d, 0.94d - (Math.Abs(offset) * 0.08d));
            item.Rotation = offset == 0 ? 0d : Math.Clamp(offset * -18d, -MaxRotation, MaxRotation);
            item.Opacity = offset == 0 ? 1d : Math.Max(MinOpacity, 0.9d - (Math.Abs(offset) * 0.14d));
            item.ZIndex = model.Items.Count - Math.Abs(offset);
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

        var offset = index - selectedIndex;
        item.IsSelected = offset == 0;
        item.X = CenterX + (offset * HorizontalStep);
        item.Y = CenterY + (Math.Abs(offset) * DepthStep);
        item.Scale = offset == 0 ? 1.08d : Math.Max(0.72d, 0.94d - (Math.Abs(offset) * 0.08d));
        item.Rotation = offset == 0 ? 0d : Math.Clamp(offset * -18d, -MaxRotation, MaxRotation);
        item.Opacity = offset == 0 ? 1d : Math.Max(MinOpacity, 0.9d - (Math.Abs(offset) * 0.14d));
        item.ZIndex = totalCount - Math.Abs(offset);
        return item;
    }
}
