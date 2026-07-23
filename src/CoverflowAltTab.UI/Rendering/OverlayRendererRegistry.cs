namespace CoverflowAltTab.UI.Rendering;

public static class OverlayRendererRegistry
{
    private static readonly Dictionary<string, Func<IOverlayRenderer>> Factories = new(StringComparer.OrdinalIgnoreCase)
    {
        ["builtin.coverflow"] = () => new CoverflowOverlayRenderer(),
        ["builtin.list"] = () => new ListOverlayRenderer(),
        ["builtin.stagemanager"] = () => new StageManagerOverlayRenderer(),
    };

    public const string DefaultRendererId = "builtin.coverflow";

    public static IReadOnlyList<string> AvailableIds => Factories.Keys.ToList();

    public static IOverlayRenderer Resolve(string? rendererId)
    {
        if (rendererId is not null && Factories.TryGetValue(rendererId, out var factory))
        {
            return factory();
        }

        return Factories[DefaultRendererId]();
    }
}
