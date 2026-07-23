using System.Windows;
using System.Windows.Media.Animation;

namespace CoverflowAltTab.UI.Motion;

public sealed class SlideCubicAnimationStrategy : IAnimationStrategy
{
    public string Id => "builtin.slide-cubic";

    public Duration Duration { get; } = new(TimeSpan.FromMilliseconds(450));

    public IEasingFunction CreateEasing() => new CubicEase { EasingMode = EasingMode.EaseInOut };
}
