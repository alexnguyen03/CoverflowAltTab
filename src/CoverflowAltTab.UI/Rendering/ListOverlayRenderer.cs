using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.UI.Rendering;

public sealed class ListOverlayRenderer : IOverlayRenderer
{
    public string Id => "builtin.list";

    public OverlayRenderModel BuildModel(SwitchSession session)
    {
        var items = new List<OverlayRenderItem>(session.Windows.Count);

        for (var index = 0; index < session.Windows.Count; index++)
        {
            var window = session.Windows[index];
            items.Add(new OverlayRenderItem
            {
                Title = window.Title,
                Subtitle = string.IsNullOrWhiteSpace(window.ProcessName) ? "Unknown" : window.ProcessName,
                IsSelected = index == session.SelectedIndex,
            });
        }

        return new OverlayRenderModel
        {
            RendererId = Id,
            HeaderTitle = "CoverflowAltTab MVP",
            HeaderSubtitle = $"Sort: {session.ActiveSortStrategyId}",
            Items = items,
            SelectedIndex = session.SelectedIndex,
        };
    }

    public void UpdateSelection(OverlayRenderModel model, int selectedIndex)
    {
        model.SelectedIndex = selectedIndex;
        for (var index = 0; index < model.Items.Count; index++)
        {
            model.Items[index].IsSelected = index == selectedIndex;
        }
    }
}
