using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.UI.Rendering;

public interface IOverlayRenderer
{
    string Id { get; }

    OverlayRenderModel BuildModel(SwitchSession session);

    void UpdateSelection(OverlayRenderModel model, int selectedIndex);
}
