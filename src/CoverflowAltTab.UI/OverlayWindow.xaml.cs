using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using CoverflowAltTab.Core.Models;
using CoverflowAltTab.UI.Rendering;

namespace CoverflowAltTab.UI;

public partial class OverlayWindow : Window
{
    // Fixed 3-slot coverflow (left/center/right), driven directly from code-behind via
    // TranslateTransform + ScaleTransform + DoubleAnimation - the same technique as the
    // CarouselDemo prototype - instead of MVVM/ItemsControl binding.
    private const double CardWidth = 460d;
    private const double MainScale = 1.5d;
    private const double SideScale = 0.72d;
    private const double HorizontalGap = 100d;
    private const double StepOffset = (CardWidth * MainScale / 2d) + HorizontalGap + (CardWidth * SideScale / 2d);
    private static readonly Duration AnimationDuration = new(TimeSpan.FromMilliseconds(450));

    // Role -1 = left, 0 = center, +1 = right, indexed as Roles[role + 1].
    private static readonly (double X, double Scale, double Opacity, int Z)[] Roles =
    {
        (-StepOffset, SideScale, 0.55, 1),
        (0d, MainScale, 1.00, 3),
        (StepOffset, SideScale, 0.55, 1),
    };

    private sealed record CoverCard(Grid Element, TranslateTransform Translate, ScaleTransform Scale);

    private CoverCard[] _cards = null!;
    private readonly int[] _cardRole = new int[3]; // which role (-1/0/+1) each card currently occupies
    private readonly nint[] _cardHandle = new nint[3];
    private int _lastSelectedIndex = -1;
    private int _lastCount;

    public OverlayWindow()
    {
        InitializeComponent();

        _cards = new[]
        {
            new CoverCard(LeftCard, LeftCardTranslate, LeftCardScale),
            new CoverCard(CenterCard, CenterCardTranslate, CenterCardScale),
            new CoverCard(RightCard, RightCardTranslate, RightCardScale),
        };
    }

    public event EventHandler<OverlayCommandRequestedEventArgs>? CommandRequested;

    public event EventHandler<OverlayDeactivatedEventArgs>? OverlayDeactivated;

    public event EventHandler? PreviewBoundsChanged;

    public nint WindowHandle => new WindowInteropHelper(this).Handle;

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);

        if (e.Key == Key.Tab)
        {
            var command = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)
                ? OverlayCommand.Previous
                : OverlayCommand.Next;

            e.Handled = true;
            CommandRequested?.Invoke(this, new OverlayCommandRequestedEventArgs(command));
            return;
        }

        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            CommandRequested?.Invoke(this, new OverlayCommandRequestedEventArgs(OverlayCommand.Commit));
            return;
        }

        if (e.Key == Key.Escape)
        {
            e.Handled = true;
            CommandRequested?.Invoke(this, new OverlayCommandRequestedEventArgs(OverlayCommand.Cancel));
        }
    }

    protected override void OnDeactivated(EventArgs e)
    {
        base.OnDeactivated(e);
        OverlayDeactivated?.Invoke(this, new OverlayDeactivatedEventArgs());
    }

    protected override void OnLocationChanged(EventArgs e)
    {
        base.OnLocationChanged(e);
        PreviewBoundsChanged?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        PreviewBoundsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ShowCoverflow(OverlayRenderModel model)
    {
        if (!model.UsesFreeformLayout || model.Items.Count == 0)
        {
            HideCoverflow();
            return;
        }

        for (var i = 0; i < _cards.Length; i++)
        {
            _cardRole[i] = i - 1;
        }

        _lastSelectedIndex = model.SelectedIndex;
        _lastCount = model.Items.Count;

        AssignContent(model);
        ApplyRolePositions(animate: false);
    }

    public void UpdateCoverflow(OverlayRenderModel model)
    {
        if (!model.UsesFreeformLayout || model.Items.Count == 0)
        {
            HideCoverflow();
            return;
        }

        if (model.Items.Count != _lastCount)
        {
            ShowCoverflow(model);
            return;
        }

        var direction = InferDirection(_lastSelectedIndex, model.SelectedIndex, model.Items.Count);
        _lastSelectedIndex = model.SelectedIndex;

        if (direction == 0)
        {
            AssignContent(model);
            ApplyRolePositions(animate: false);
            return;
        }

        RotateRoles(model, direction);
        ApplyRolePositions(animate: true);
    }

    public IReadOnlyList<nint> GetSessionWindowHandles()
    {
        return _cardHandle.Where(handle => handle != 0).Distinct().ToList();
    }

    public bool TryGetPreviewBounds(nint windowHandle, out WindowBounds bounds)
    {
        bounds = default;

        if (!IsLoaded || !IsVisible)
        {
            return false;
        }

        var cardIndex = Array.IndexOf(_cardHandle, windowHandle);
        if (cardIndex < 0)
        {
            return false;
        }

        var surface = _cards[cardIndex].Element;
        if (surface.ActualWidth <= 0 || surface.ActualHeight <= 0)
        {
            return false;
        }

        var toWindow = surface.TransformToAncestor(this);
        var topLeftWindow = toWindow.Transform(new Point(0, 0));
        var bottomRightWindow = toWindow.Transform(new Point(surface.ActualWidth, surface.ActualHeight));

        var source = PresentationSource.FromVisual(this);
        if (source?.CompositionTarget is null)
        {
            return false;
        }

        var transform = source.CompositionTarget.TransformToDevice;
        var topLeft = transform.Transform(topLeftWindow);
        var bottomRight = transform.Transform(bottomRightWindow);

        bounds = new WindowBounds(
            (int)Math.Round(topLeft.X),
            (int)Math.Round(topLeft.Y),
            (int)Math.Round(bottomRight.X),
            (int)Math.Round(bottomRight.Y));
        return true;
    }

    // Recomputes which window each of the 3 cards shows, keyed off its current role
    // (offset relative to the selected window). Used on first show and whenever the
    // selection changed in a way that is not a simple +1/-1 step.
    private void AssignContent(OverlayRenderModel model)
    {
        var count = model.Items.Count;

        for (var i = 0; i < _cards.Length; i++)
        {
            var index = Wrap(model.SelectedIndex + _cardRole[i], count);
            _cardHandle[i] = model.Items[index].WindowHandle;
        }
    }

    // Shifts every card's role by one step. The card whose role would fall outside the
    // visible [-1, 0, +1] range is the one that just scrolled off screen: its content is
    // replaced with the window that just entered the trio, and it is snapped just outside
    // the frame it is about to slide into so the transition reads as a new card entering
    // rather than a teleport across the stage.
    private void RotateRoles(OverlayRenderModel model, int direction)
    {
        var count = model.Items.Count;

        for (var i = 0; i < _cards.Length; i++)
        {
            var newRole = _cardRole[i] - direction;

            if (newRole is < -1 or > 1)
            {
                newRole = direction > 0 ? 1 : -1;

                var enteringIndex = Wrap(model.SelectedIndex + newRole, count);
                _cardHandle[i] = model.Items[enteringIndex].WindowHandle;

                var role = Roles[newRole + 1];
                var entryX = role.X + (Math.Sign(direction) * StepOffset);
                ApplyTransform(i, entryX, role.Scale, 0d, role.Z, animate: false);
            }

            _cardRole[i] = newRole;
        }
    }

    private void HideCoverflow()
    {
        Array.Clear(_cardHandle);

        foreach (var card in _cards)
        {
            card.Translate.BeginAnimation(TranslateTransform.XProperty, null);
            card.Scale.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            card.Scale.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            card.Element.BeginAnimation(UIElement.OpacityProperty, null);
            card.Element.Opacity = 0d;
        }
    }

    private void ApplyRolePositions(bool animate)
    {
        for (var i = 0; i < _cards.Length; i++)
        {
            var role = Roles[_cardRole[i] + 1];
            ApplyTransform(i, role.X, role.Scale, role.Opacity, role.Z, animate);
        }
    }

    private void ApplyTransform(int cardIndex, double x, double scale, double opacity, int zIndex, bool animate)
    {
        var card = _cards[cardIndex];
        Panel.SetZIndex(card.Element, zIndex);

        if (!animate)
        {
            card.Translate.BeginAnimation(TranslateTransform.XProperty, null);
            card.Scale.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            card.Scale.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            card.Element.BeginAnimation(UIElement.OpacityProperty, null);

            card.Translate.X = x;
            card.Scale.ScaleX = card.Scale.ScaleY = scale;
            card.Element.Opacity = opacity;
            return;
        }

        var ease = new CubicEase { EasingMode = EasingMode.EaseInOut };

        card.Translate.BeginAnimation(TranslateTransform.XProperty,
            new DoubleAnimation(x, AnimationDuration) { EasingFunction = ease });
        card.Scale.BeginAnimation(ScaleTransform.ScaleXProperty,
            new DoubleAnimation(scale, AnimationDuration) { EasingFunction = ease });
        card.Scale.BeginAnimation(ScaleTransform.ScaleYProperty,
            new DoubleAnimation(scale, AnimationDuration) { EasingFunction = ease });
        card.Element.BeginAnimation(UIElement.OpacityProperty,
            new DoubleAnimation(opacity, AnimationDuration) { EasingFunction = ease });
    }

    private static int InferDirection(int oldIndex, int newIndex, int count)
    {
        if (oldIndex == newIndex || count <= 1)
        {
            return 0;
        }

        var diff = newIndex - oldIndex;

        if (diff == 1 || diff == -(count - 1))
        {
            return 1;
        }

        if (diff == -1 || diff == count - 1)
        {
            return -1;
        }

        return 0;
    }

    private static int Wrap(int index, int count) => ((index % count) + count) % count;
}
