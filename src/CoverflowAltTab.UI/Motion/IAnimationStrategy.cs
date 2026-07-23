using System.Windows;
using System.Windows.Media.Animation;

namespace CoverflowAltTab.UI.Motion;

public interface IAnimationStrategy
{
    string Id { get; }

    Duration Duration { get; }

    IEasingFunction? CreateEasing();
}
