using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using CoverflowAltTab.Core.Models;

namespace CoverflowAltTab.UI;

public partial class OverlayWindow : Window
{
    public OverlayWindow()
    {
        InitializeComponent();
        CardsItemsControl.LayoutUpdated += OnCardsLayoutUpdated;
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

    public IReadOnlyList<nint> GetLiveThumbnailWindowHandles()
    {
        return CardsItemsControl.ItemsSource is IEnumerable<WindowListItemViewModel> items
            ? items.Where(item => item.HasLiveThumbnail).Select(item => item.WindowHandle).ToList()
            : Array.Empty<nint>();
    }

    public bool TryGetPreviewBounds(nint windowHandle, out WindowBounds bounds)
    {
        bounds = default;

        if (!IsLoaded || !IsVisible)
        {
            return false;
        }

        if (CardsItemsControl.ItemsSource is not IEnumerable<WindowListItemViewModel> items)
        {
            return false;
        }

        var item = items.FirstOrDefault(candidate => candidate.WindowHandle == windowHandle);
        if (item is null)
        {
            return false;
        }

        if (CardsItemsControl.ItemContainerGenerator.ContainerFromItem(item) is not ContentPresenter container)
        {
            return false;
        }

        container.ApplyTemplate();

        if (container.ContentTemplate?.FindName("PreviewSurface", container) is not FrameworkElement surface ||
            surface.ActualWidth <= 0 || surface.ActualHeight <= 0)
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

    private void OnCardsLayoutUpdated(object? sender, EventArgs e)
    {
        if (IsVisible)
        {
            PreviewBoundsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
