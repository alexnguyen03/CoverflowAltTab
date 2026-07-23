using System.ComponentModel;
using System.Windows.Threading;
using CoverflowAltTab.Extensibility;
using CoverflowAltTab.Platform;

namespace CoverflowAltTab.Host.Debug;

public sealed class DebugWindowController
{
    private readonly DebugWindowViewModel _viewModel = new();
    private readonly DebugWindow _window;
    private readonly Dispatcher _dispatcher;

    public DebugWindowController()
    {
        _window = new DebugWindow
        {
            DataContext = _viewModel,
        };
        _dispatcher = _window.Dispatcher;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    public event EventHandler<string>? RendererSelected;

    public event EventHandler<string>? AnimationSelected;

    public DebugWindow Window => _window;

    public void Show()
    {
        if (!_window.IsVisible)
        {
            _window.Show();
        }
    }

    public void SetAppStatus(string status)
    {
        _dispatcher.BeginInvoke(() => _viewModel.AppStatus = status);
    }

    public void SetHotkeyStatus(string status)
    {
        _dispatcher.BeginInvoke(() => _viewModel.HotkeyStatus = status);
    }

    public void SetSessionStatus(string status)
    {
        _dispatcher.BeginInvoke(() => _viewModel.SessionStatus = status);
    }

    public void SetLastAction(string action)
    {
        _dispatcher.BeginInvoke(() => _viewModel.LastAction = action);
    }

    public void AddKeyEvent(KeyEventRecord record)
    {
        _dispatcher.BeginInvoke(() =>
        {
            _viewModel.AddKey(new DebugKeyEventViewModel(
                record.Timestamp.ToString("HH:mm:ss.fff"),
                record.Kind.ToString(),
                record.ModifiersDisplay));
        });
    }

    public void SetExtensions(IEnumerable<ExtensionLoadResult> results)
    {
        _dispatcher.BeginInvoke(() =>
        {
            _viewModel.SetExtensions(results.Select(result =>
                new DebugExtensionViewModel(
                    result.Success ? "Loaded" : "Failed",
                    string.IsNullOrWhiteSpace(result.ExtensionName) ? result.ExtensionId : result.ExtensionName,
                    result.Version,
                    string.IsNullOrWhiteSpace(result.Description) ? "No description" : result.Description,
                    $"Filters: {result.RegisteredWindowFilters}, Sorts: {result.RegisteredSortStrategies}, Dismiss: {result.RegisteredOverlayDismissBehaviors}",
                    result.Message)));
        });
    }

    public void InitializeOverlaySettings(
        IEnumerable<string> availableRendererIds,
        IEnumerable<string> availableAnimationIds,
        string selectedRendererId,
        string selectedAnimationId)
    {
        _dispatcher.BeginInvoke(() =>
        {
            foreach (var id in availableRendererIds)
            {
                _viewModel.AvailableRendererIds.Add(id);
            }

            foreach (var id in availableAnimationIds)
            {
                _viewModel.AvailableAnimationIds.Add(id);
            }

            _viewModel.SelectedRendererId = selectedRendererId;
            _viewModel.SelectedAnimationId = selectedAnimationId;
        });
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(DebugWindowViewModel.SelectedRendererId):
                if (_viewModel.SelectedRendererId is { } rendererId)
                {
                    RendererSelected?.Invoke(this, rendererId);
                }

                break;
            case nameof(DebugWindowViewModel.SelectedAnimationId):
                if (_viewModel.SelectedAnimationId is { } animationId)
                {
                    AnimationSelected?.Invoke(this, animationId);
                }

                break;
        }
    }
}
