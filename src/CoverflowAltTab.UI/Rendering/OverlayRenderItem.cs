namespace CoverflowAltTab.UI.Rendering;

public sealed class OverlayRenderItem
{
    public required string Title { get; init; }

    public required string Subtitle { get; init; }

    public required nint WindowHandle { get; init; }

    public bool IsSelected { get; set; }

    public double X { get; set; }

    public double Y { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }

    public double Scale { get; set; } = 1d;

    public double Rotation { get; set; }

    public double Opacity { get; set; } = 1d;

    public int ZIndex { get; set; }
}
