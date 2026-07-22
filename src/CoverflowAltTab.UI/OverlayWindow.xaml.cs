using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.UI;

public partial class OverlayWindow : Window
{
    public OverlayWindow()
    {
        InitializeComponent();
        SelectedPreviewHost.LayoutUpdated += OnSelectedPreviewHostLayoutUpdated;
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

    public bool TryGetPreviewBounds(out WindowBounds bounds)
    {
        bounds = default;
        if (!IsLoaded || !IsVisible || SelectedPreviewSurface.ActualWidth <= 0 || SelectedPreviewSurface.ActualHeight <= 0)
        {
            return false;
        }

        var relative = SelectedPreviewSurface.TransformToAncestor(this).Transform(new Point(0, 0));
        var source = PresentationSource.FromVisual(this);
        if (source?.CompositionTarget is null)
        {
            return false;
        }

        var transform = source.CompositionTarget.TransformToDevice;
        var topLeft = transform.Transform(relative);
        var bottomRight = transform.Transform(new Point(relative.X + SelectedPreviewSurface.ActualWidth, relative.Y + SelectedPreviewSurface.ActualHeight));

        bounds = new WindowBounds(
            (int)Math.Round(topLeft.X),
            (int)Math.Round(topLeft.Y),
            (int)Math.Round(bottomRight.X),
            (int)Math.Round(bottomRight.Y));
        return true;
    }

    private void OnSelectedPreviewHostLayoutUpdated(object? sender, EventArgs e)
    {
        if (IsVisible)
        {
            PreviewBoundsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
