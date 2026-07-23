using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.UI.Rendering;

public sealed class CoverflowOverlayRenderer : IOverlayRenderer
{
    private const double StageWidth = 1400d;
    private const double StageHeight = 650d;
    private const double CardWidth = 400d;
    private const double CardHeight = 260d;
    private const double CenterX = StageWidth / 2d;
    private const double CenterY = StageHeight / 2d;
    private const double MainScale = 1.5d;
    private const double SideScale = 0.72d;
    private const double HorizontalGap = 60d;
    private const double StepOffset = (CardWidth * MainScale / 2d) + HorizontalGap + (CardWidth * SideScale / 2d);

    public string Id => "builtin.coverflow";

    public OverlayRenderModel BuildModel(SwitchSession session)
    {
        var items = new List<OverlayRenderItem>(session.Windows.Count);

        for (var index = 0; index < session.Windows.Count; index++)
        {
            var item = new OverlayRenderItem
            {
                Title = windowTitle(session.Windows[index].Title),
                Subtitle = string.Empty,
                WindowHandle = session.Windows[index].Handle,
            };

            ApplySlotLayout(item, GetCircularOffset(index, session.SelectedIndex, session.Windows.Count), session.Windows.Count);
            items.Add(item);
        }

        return new OverlayRenderModel
        {
            RendererId = Id,
            HeaderTitle = string.Empty,
            HeaderSubtitle = string.Empty,
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
            ApplySlotLayout(model.Items[index], GetCircularOffset(index, selectedIndex, model.Items.Count), model.Items.Count);
        }
    }

    private static void ApplySlotLayout(OverlayRenderItem item, int circularOffset, int totalCount)
    {
        var distance = Math.Abs(circularOffset);
        var sign = Math.Sign(circularOffset);

        item.IsSelected = circularOffset == 0;
        item.Width = CardWidth;
        item.Height = CardHeight;
        item.X = CenterX + (sign * StepOffset * distance) - (CardWidth / 2d);
        item.Y = CenterY - (CardHeight / 2d);
        item.Scale = distance == 0 ? MainScale : SideScale;
        item.Rotation = 0d;
        item.Opacity = 1d;
        item.ZIndex = totalCount - distance;
    }

    private static int GetCircularOffset(int itemIndex, int selectedIndex, int count)
    {
        if (count <= 1)
        {
            return 0;
        }

        var rawOffset = itemIndex - selectedIndex;
        var half = count / 2;

        if (rawOffset > half)
        {
            rawOffset -= count;
        }
        else if (rawOffset < -half)
        {
            rawOffset += count;
        }

        return rawOffset;
    }

    private static string windowTitle(string title)
    {
        return string.IsNullOrWhiteSpace(title) ? "Window" : title;
    }
}
