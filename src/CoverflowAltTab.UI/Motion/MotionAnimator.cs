using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CoverflowAltTab.UI.Motion;

public static class MotionAnimator
{
    public static readonly DependencyProperty AnimatedLeftProperty =
        DependencyProperty.RegisterAttached(
            "AnimatedLeft",
            typeof(double),
            typeof(MotionAnimator),
            new PropertyMetadata(0d, OnAnimatedLeftChanged));

    public static readonly DependencyProperty AnimatedTopProperty =
        DependencyProperty.RegisterAttached(
            "AnimatedTop",
            typeof(double),
            typeof(MotionAnimator),
            new PropertyMetadata(0d, OnAnimatedTopChanged));

    public static readonly DependencyProperty AnimatedScaleProperty =
        DependencyProperty.RegisterAttached(
            "AnimatedScale",
            typeof(double),
            typeof(MotionAnimator),
            new PropertyMetadata(1d, OnAnimatedScaleChanged));

    public static readonly DependencyProperty AnimatedSkewYProperty =
        DependencyProperty.RegisterAttached(
            "AnimatedSkewY",
            typeof(double),
            typeof(MotionAnimator),
            new PropertyMetadata(0d, OnAnimatedSkewYChanged));

    public static readonly DependencyProperty AnimatedOpacityProperty =
        DependencyProperty.RegisterAttached(
            "AnimatedOpacity",
            typeof(double),
            typeof(MotionAnimator),
            new PropertyMetadata(1d, OnAnimatedOpacityChanged));

    public static readonly DependencyProperty AnimatedWidthProperty =
        DependencyProperty.RegisterAttached(
            "AnimatedWidth",
            typeof(double),
            typeof(MotionAnimator),
            new PropertyMetadata(double.NaN, OnAnimatedWidthChanged));

    public static readonly DependencyProperty AnimatedHeightProperty =
        DependencyProperty.RegisterAttached(
            "AnimatedHeight",
            typeof(double),
            typeof(MotionAnimator),
            new PropertyMetadata(double.NaN, OnAnimatedHeightChanged));

    public static void SetAnimatedLeft(DependencyObject element, double value) => element.SetValue(AnimatedLeftProperty, value);

    public static double GetAnimatedLeft(DependencyObject element) => (double)element.GetValue(AnimatedLeftProperty);

    public static void SetAnimatedTop(DependencyObject element, double value) => element.SetValue(AnimatedTopProperty, value);

    public static double GetAnimatedTop(DependencyObject element) => (double)element.GetValue(AnimatedTopProperty);

    public static void SetAnimatedScale(DependencyObject element, double value) => element.SetValue(AnimatedScaleProperty, value);

    public static double GetAnimatedScale(DependencyObject element) => (double)element.GetValue(AnimatedScaleProperty);

    public static void SetAnimatedSkewY(DependencyObject element, double value) => element.SetValue(AnimatedSkewYProperty, value);

    public static double GetAnimatedSkewY(DependencyObject element) => (double)element.GetValue(AnimatedSkewYProperty);

    public static void SetAnimatedOpacity(DependencyObject element, double value) => element.SetValue(AnimatedOpacityProperty, value);

    public static double GetAnimatedOpacity(DependencyObject element) => (double)element.GetValue(AnimatedOpacityProperty);

    public static void SetAnimatedWidth(DependencyObject element, double value) => element.SetValue(AnimatedWidthProperty, value);

    public static double GetAnimatedWidth(DependencyObject element) => (double)element.GetValue(AnimatedWidthProperty);

    public static void SetAnimatedHeight(DependencyObject element, double value) => element.SetValue(AnimatedHeightProperty, value);

    public static double GetAnimatedHeight(DependencyObject element) => (double)element.GetValue(AnimatedHeightProperty);

    private static void OnAnimatedLeftChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not UIElement element)
        {
            return;
        }

        AnimateDouble(
            element,
            Canvas.LeftProperty,
            e.NewValue,
            fallbackSetter: value => Canvas.SetLeft(element, value));
    }

    private static void OnAnimatedTopChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not UIElement element)
        {
            return;
        }

        AnimateDouble(
            element,
            Canvas.TopProperty,
            e.NewValue,
            fallbackSetter: value => Canvas.SetTop(element, value));
    }

    private static void OnAnimatedScaleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not FrameworkElement element)
        {
            return;
        }

        var scaleTransform = EnsureScaleTransform(element);
        AnimateDouble(scaleTransform, ScaleTransform.ScaleXProperty, e.NewValue);
        AnimateDouble(scaleTransform, ScaleTransform.ScaleYProperty, e.NewValue);
    }

    private static void OnAnimatedSkewYChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not FrameworkElement element)
        {
            return;
        }

        var skewTransform = EnsureSkewTransform(element);
        AnimateDouble(skewTransform, SkewTransform.AngleYProperty, e.NewValue);
    }

    private static void OnAnimatedOpacityChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not UIElement element)
        {
            return;
        }

        AnimateDouble(element, UIElement.OpacityProperty, e.NewValue, fallbackSetter: value => element.Opacity = value);
    }

    private static void OnAnimatedWidthChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not FrameworkElement element)
        {
            return;
        }

        AnimateDouble(element, FrameworkElement.WidthProperty, e.NewValue, fallbackSetter: value => element.Width = value);
    }

    private static void OnAnimatedHeightChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not FrameworkElement element)
        {
            return;
        }

        AnimateDouble(element, FrameworkElement.HeightProperty, e.NewValue, fallbackSetter: value => element.Height = value);
    }

    private static void AnimateDouble(
        DependencyObject target,
        DependencyProperty property,
        object? newValue,
        Action<double>? fallbackSetter = null)
    {
        if (newValue is not double value)
        {
            return;
        }

        if (target is not Animatable animatable)
        {
            fallbackSetter?.Invoke(value);
            return;
        }

        var strategy = AnimationStrategyRegistry.Current;
        var animation = new DoubleAnimation
        {
            To = value,
            Duration = strategy.Duration,
            EasingFunction = strategy.CreateEasing(),
        };

        animatable.BeginAnimation(property, animation, HandoffBehavior.SnapshotAndReplace);
    }

    private static ScaleTransform EnsureScaleTransform(FrameworkElement element)
    {
        var transformGroup = EnsureTransformGroup(element);
        var scaleTransform = transformGroup.Children.OfType<ScaleTransform>().FirstOrDefault();
        if (scaleTransform is not null)
        {
            return scaleTransform;
        }

        scaleTransform = new ScaleTransform(1d, 1d);
        transformGroup.Children.Insert(0, scaleTransform);
        return scaleTransform;
    }

    private static SkewTransform EnsureSkewTransform(FrameworkElement element)
    {
        var transformGroup = EnsureTransformGroup(element);
        var skewTransform = transformGroup.Children.OfType<SkewTransform>().FirstOrDefault();
        if (skewTransform is not null)
        {
            return skewTransform;
        }

        skewTransform = new SkewTransform(0d, 0d);
        transformGroup.Children.Add(skewTransform);
        return skewTransform;
    }

    private static TransformGroup EnsureTransformGroup(FrameworkElement element)
    {
        if (element.RenderTransform is TransformGroup transformGroup)
        {
            return transformGroup;
        }

        transformGroup = new TransformGroup();
        if (element.RenderTransform is { } existingTransform &&
            !(existingTransform is MatrixTransform matrixTransform && matrixTransform.Matrix.IsIdentity))
        {
            transformGroup.Children.Add(existingTransform);
        }

        element.RenderTransform = transformGroup;
        return transformGroup;
    }
}
