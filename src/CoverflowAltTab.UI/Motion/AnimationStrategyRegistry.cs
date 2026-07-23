namespace CoverflowAltTab.UI.Motion;

public static class AnimationStrategyRegistry
{
    private static readonly Dictionary<string, Func<IAnimationStrategy>> Factories = new(StringComparer.OrdinalIgnoreCase)
    {
        ["builtin.slide-cubic"] = () => new SlideCubicAnimationStrategy(),
        ["builtin.snappy"] = () => new SnappyAnimationStrategy(),
    };

    public const string DefaultAnimationId = "builtin.slide-cubic";

    private static IAnimationStrategy _current = Factories[DefaultAnimationId]();

    public static IAnimationStrategy Current => _current;

    public static IReadOnlyList<string> AvailableIds => Factories.Keys.ToList();

    public static void SetCurrent(string? animationId)
    {
        _current = animationId is not null && Factories.TryGetValue(animationId, out var factory)
            ? factory()
            : Factories[DefaultAnimationId]();
    }
}
