using System.Windows;
using System.Windows.Media.Animation;

namespace CoverflowAltTab.UI.Motion;

public sealed class SnappyAnimationStrategy : IAnimationStrategy
{
    public string Id => "builtin.snappy";

    public Duration Duration { get; } = new(TimeSpan.FromMilliseconds(250));

    public IEasingFunction CreateEasing() => new QuadraticEase { EasingMode = EasingMode.EaseOut };
}
