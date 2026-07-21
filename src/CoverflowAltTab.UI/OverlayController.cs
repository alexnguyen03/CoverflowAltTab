using System.Windows.Threading;
using CoverflowAltTab.Core.Models;
using CoverflowAltTab.UI.Rendering;

namespace CoverflowAltTab.UI;

public sealed class OverlayController : IDisposable
{
    private readonly IOverlayRenderer _renderer;
    private readonly OverlayViewModel _viewModel = new();
    private readonly OverlayWindow _window;
    private OverlayRenderModel? _currentModel;

    public OverlayController(IOverlayRenderer renderer)
    {
        _renderer = renderer;
        _window = new OverlayWindow
        {
            DataContext = _viewModel,
        };
        _window.CommandRequested += OnCommandRequested;
        _window.OverlayDeactivated += OnOverlayDeactivated;
        _window.PreviewBoundsChanged += OnPreviewBoundsChanged;
    }

    public event EventHandler<OverlayCommandRequestedEventArgs>? CommandRequested;

    public event EventHandler<OverlayDeactivatedEventArgs>? OverlayDeactivated;

    public event EventHandler? PreviewBoundsChanged;

    public void ShowSession(SwitchSession session)
    {
        _currentModel = _renderer.BuildModel(session);
        _viewModel.ShowModel(_currentModel);

        if (!_window.IsVisible)
        {
            _window.Show();
        }

        _window.Dispatcher.BeginInvoke(() =>
        {
            _window.Activate();
            _window.Focus();
        }, DispatcherPriority.Input);
    }

    public void UpdateSession(SwitchSession session)
    {
        if (_currentModel is null)
        {
            _currentModel = _renderer.BuildModel(session);
            _viewModel.ShowModel(_currentModel);
        }
        else
        {
            _renderer.UpdateSelection(_currentModel, session.SelectedIndex);
            _viewModel.UpdateSelection(_currentModel);
        }

        _window.Dispatcher.BeginInvoke(() =>
        {
            _window.Activate();
            _window.Focus();
        }, DispatcherPriority.Input);
    }

    public void HideOverlay()
    {
        _currentModel = null;
        _viewModel.Clear();
        _window.Hide();
    }

    public void Dispose()
    {
        _window.CommandRequested -= OnCommandRequested;
        _window.OverlayDeactivated -= OnOverlayDeactivated;
        _window.PreviewBoundsChanged -= OnPreviewBoundsChanged;
        _window.Close();
    }

    public bool TryGetPreviewBounds(out WindowBounds bounds)
    {
        return _window.TryGetPreviewBounds(out bounds);
    }

    public nint WindowHandle => _window.WindowHandle;

    private void OnCommandRequested(object? sender, OverlayCommandRequestedEventArgs e)
    {
        CommandRequested?.Invoke(this, e);
    }

    private void OnOverlayDeactivated(object? sender, OverlayDeactivatedEventArgs e)
    {
        OverlayDeactivated?.Invoke(this, e);
    }

    private void OnPreviewBoundsChanged(object? sender, EventArgs e)
    {
        PreviewBoundsChanged?.Invoke(this, EventArgs.Empty);
    }
}
