using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.UI.Rendering;

public sealed class CoverflowOverlayRenderer : IOverlayRenderer
{
    private const double StageWidth = 1080d;
    private const double StageHeight = 420d;
    private const double SelectedWidth = 520d;
    private const double SelectedHeight = 300d;
    private const double SideWidth = 320d;
    private const double SideHeight = 186d;
    private const double CenterX = (StageWidth - SelectedWidth) / 2d;
    private const double CenterY = 56d;
    private const double RadiusX = 360d;
    private const double RadiusY = 88d;
    private const double StepAngle = 0.62d;
    private const double MaxAngle = 1.92d;

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
        item.IsSelected = circularOffset == 0;

        if (circularOffset == 0)
        {
            item.X = CenterX;
            item.Y = CenterY;
            item.Width = SelectedWidth;
            item.Height = SelectedHeight;
            item.Scale = 1d;
            item.Rotation = 0d;
            item.Opacity = 0.16d;
            item.ZIndex = totalCount + 10;
            return;
        }

        var sign = Math.Sign(circularOffset);
        var distance = Math.Min(Math.Abs(circularOffset), 4);
        var angle = Math.Min(MaxAngle, distance * StepAngle);
        var normalizedDepth = 1d - Math.Cos(angle);
        var scale = Math.Max(0.54d, 0.96d - (distance * 0.12d));

        item.Width = SideWidth;
        item.Height = SideHeight;
        item.X = CenterX + (Math.Sin(angle) * RadiusX * sign) + (sign * 48d);
        item.Y = CenterY + 34d + (normalizedDepth * RadiusY);
        item.Scale = scale;
        item.Rotation = -sign * Math.Min(60d, 20d + (distance * 10d));
        item.Opacity = Math.Max(0.18d, 0.88d - (distance * 0.16d));
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
