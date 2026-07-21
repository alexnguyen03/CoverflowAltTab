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
    }

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
}
