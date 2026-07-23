using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.UI.Rendering;

// Vertical rail of small thumbnails for every non-selected window on the left edge,
// plus a single large live preview of the selected window on the right - modeled after
// macOS Stage Manager, where the side strip only ever shows the *other* windows.
public sealed class StageManagerOverlayRenderer : IOverlayRenderer
{
    private const double StageWidth = 1400d;
    private const double StageHeight = 760d;
    private const double RailX = 40d;
    private const double RailItemWidth = 220d;
    private const double RailItemHeight = 140d;
    private const double RailSpacing = 16d;
    private const double PreviewGap = 60d;
    private const double PreviewX = RailX + RailItemWidth + PreviewGap;
    private const double PreviewWidth = StageWidth - PreviewX - 40d;
    private const double PreviewHeight = 680d;
    private const double PreviewY = (StageHeight - PreviewHeight) / 2d;

    public string Id => "builtin.stagemanager";

    public OverlayRenderModel BuildModel(SwitchSession session)
    {
        var items = new List<OverlayRenderItem>(session.Windows.Count);

        for (var index = 0; index < session.Windows.Count; index++)
        {
            var window = session.Windows[index];
            items.Add(new OverlayRenderItem
            {
                Title = string.IsNullOrWhiteSpace(window.Title) ? "Window" : window.Title,
                Subtitle = string.IsNullOrWhiteSpace(window.ProcessName) ? "Unknown" : window.ProcessName,
                WindowHandle = window.Handle,
            });
        }

        var model = new OverlayRenderModel
        {
            RendererId = Id,
            HeaderTitle = string.Empty,
            HeaderSubtitle = string.Empty,
            Items = items,
            SelectedIndex = session.SelectedIndex,
            LayoutKind = OverlayLayoutKind.StageManager,
            StageWidth = StageWidth,
            StageHeight = StageHeight,
        };

        ApplyLayout(model);
        return model;
    }

    public void UpdateSelection(OverlayRenderModel model, int selectedIndex)
    {
        model.SelectedIndex = selectedIndex;
        ApplyLayout(model);
    }

    private static void ApplyLayout(OverlayRenderModel model)
    {
        var items = model.Items;
        var selectedIndex = model.SelectedIndex;
        var railOrder = new List<int>(items.Count);

        for (var index = 0; index < items.Count; index++)
        {
            if (index != selectedIndex)
            {
                railOrder.Add(index);
            }
        }

        var railCount = railOrder.Count;
        var totalRailHeight = railCount == 0
            ? 0d
            : (railCount * RailItemHeight) + ((railCount - 1) * RailSpacing);
        var railStartY = Math.Max(24d, (StageHeight - totalRailHeight) / 2d);

        for (var railSlot = 0; railSlot < railCount; railSlot++)
        {
            var item = items[railOrder[railSlot]];
            item.IsSelected = false;
            item.X = RailX;
            item.Y = railStartY + (railSlot * (RailItemHeight + RailSpacing));
            item.Width = RailItemWidth;
            item.Height = RailItemHeight;
            item.Scale = 1d;
            item.Rotation = 0d;
            item.Opacity = 0.85d;
            item.ZIndex = 1;
        }

        if (selectedIndex >= 0 && selectedIndex < items.Count)
        {
            var selected = items[selectedIndex];
            selected.IsSelected = true;
            selected.X = PreviewX;
            selected.Y = PreviewY;
            selected.Width = PreviewWidth;
            selected.Height = PreviewHeight;
            selected.Scale = 1d;
            selected.Rotation = 0d;
            selected.Opacity = 1d;
            selected.ZIndex = 10;
        }
    }
}
