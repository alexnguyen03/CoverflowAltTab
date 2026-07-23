namespace CoverflowAltTab.UI.Configuration;

public sealed record AppSettings
{
    public string RendererId { get; init; } = "builtin.coverflow";

    public string AnimationId { get; init; } = "builtin.slide-cubic";
}
