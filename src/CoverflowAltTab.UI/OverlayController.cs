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
    }

    public event EventHandler<OverlayCommandRequestedEventArgs>? CommandRequested;

    public event EventHandler<OverlayDeactivatedEventArgs>? OverlayDeactivated;

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
            _viewModel.UpdateSelection(session.SelectedIndex);
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
        _window.Close();
    }

    private void OnCommandRequested(object? sender, OverlayCommandRequestedEventArgs e)
    {
        CommandRequested?.Invoke(this, e);
    }

    private void OnOverlayDeactivated(object? sender, OverlayDeactivatedEventArgs e)
    {
        OverlayDeactivated?.Invoke(this, e);
    }
}
