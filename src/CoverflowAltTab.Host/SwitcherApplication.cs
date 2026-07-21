using CoverflowAltTab.Core.Abstractions;
using CoverflowAltTab.Core.Filters;
using CoverflowAltTab.Core.Models;
using CoverflowAltTab.Core.Services;
using CoverflowAltTab.Core.Sorting;
using CoverflowAltTab.Extensibility;
using CoverflowAltTab.Host.Adapters;
using CoverflowAltTab.Host.Debug;
using CoverflowAltTab.Platform;
using CoverflowAltTab.UI;
using System.Windows;

namespace CoverflowAltTab.Host;

public sealed class SwitcherApplication : IDisposable
{
    private const string? PreferredSortStrategyId = null;

    private readonly SessionController _sessionController;
    private readonly IWindowEnumerationService _windowEnumerationService;
    private readonly IForegroundWindowService _foregroundWindowService;
    private readonly IWindowActivationService _windowActivationService;
    private readonly OverlayController _overlayController;
    private readonly IHotkeyService _hotkeyService;
    private readonly IKeyboardMonitorService _keyboardMonitorService;
    private readonly ExtensionRegistry _extensionRegistry;
    private readonly ILogger _logger;
    private readonly int _hostProcessId;
    private readonly DebugWindowController _debugWindowController;
    private readonly DefaultStableSortStrategy _defaultSortStrategy = new();

    public SwitcherApplication(
        SessionController sessionController,
        IWindowEnumerationService windowEnumerationService,
        IForegroundWindowService foregroundWindowService,
        IWindowActivationService windowActivationService,
        OverlayController overlayController,
        IHotkeyService hotkeyService,
        IKeyboardMonitorService keyboardMonitorService,
        ExtensionRegistry extensionRegistry,
        ILogger logger,
        int hostProcessId,
        DebugWindowController debugWindowController)
    {
        _sessionController = sessionController;
        _windowEnumerationService = windowEnumerationService;
        _foregroundWindowService = foregroundWindowService;
        _windowActivationService = windowActivationService;
        _overlayController = overlayController;
        _hotkeyService = hotkeyService;
        _keyboardMonitorService = keyboardMonitorService;
        _extensionRegistry = extensionRegistry;
        _logger = logger;
        _hostProcessId = hostProcessId;
        _debugWindowController = debugWindowController;
    }

    public void Start()
    {
        _hotkeyService.Triggered += OnHotkeyTriggered;
        _keyboardMonitorService.KeyEventReceived += OnKeyEventReceived;
        _overlayController.CommandRequested += OnOverlayCommandRequested;
        _overlayController.OverlayDeactivated += OnOverlayDeactivated;
        _hotkeyService.Start();
        _keyboardMonitorService.Start();
        _debugWindowController.SetHotkeyStatus("Registered: Ctrl + Shift + Space");
        _logger.Info("Registered Ctrl+Shift+Space development trigger.");
    }

    public void Dispose()
    {
        _hotkeyService.Triggered -= OnHotkeyTriggered;
        _keyboardMonitorService.KeyEventReceived -= OnKeyEventReceived;
        _overlayController.CommandRequested -= OnOverlayCommandRequested;
        _overlayController.OverlayDeactivated -= OnOverlayDeactivated;
        _hotkeyService.Stop();
        _keyboardMonitorService.Stop();
        _overlayController.Dispose();
    }

    private void OnHotkeyTriggered(object? sender, EventArgs e)
    {
        _logger.Info("Hotkey trigger received.");
        _debugWindowController.SetLastAction("Hotkey triggered");
        if (_sessionController.State != SwitchSessionState.Idle)
        {
            _logger.Debug("Ignored trigger because a session is already open.");
            _debugWindowController.SetSessionStatus("Open");
            return;
        }

        var originalForegroundWindowHandle = _foregroundWindowService.GetForegroundWindowHandle();
        var rawWindows = _windowEnumerationService.EnumerateWindows();
        var builtInFilter = new BuiltInWindowFilter(_hostProcessId);
        LogBuiltInFilterSummary(rawWindows, builtInFilter);
        var filters = ResolveFilters(builtInFilter);
        var sortRule = ResolveSortRule();

        var result = _sessionController.OpenSession(rawWindows, originalForegroundWindowHandle, filters, sortRule);
        _logger.Info(
            $"Session open attempt: raw={result.PipelineResult.RawCount}, filtered={result.PipelineResult.FilteredCount}, sort={result.PipelineResult.SortStrategyId}.");

        if (!result.Success || result.Session is null)
        {
            _logger.Info($"Session open aborted: {result.Reason}");
            _debugWindowController.SetLastAction($"Open aborted: {result.Reason}");
            return;
        }

        _logger.Info($"Overlay shown for session {result.Session.SessionId}.");
        _debugWindowController.SetSessionStatus($"Open ({result.Session.Windows.Count} windows)");
        _debugWindowController.SetLastAction($"Overlay shown: {result.Session.SessionId}");
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            try
            {
                _overlayController.ShowSession(result.Session);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to show overlay.", ex);
                _debugWindowController.SetLastAction("Overlay show failed");
                _debugWindowController.SetSessionStatus("Idle");
                _sessionController.CancelCurrentSession();
            }
        });
    }

    private void OnKeyEventReceived(object? sender, KeyEventRecord e)
    {
        _debugWindowController.AddKeyEvent(e);
    }

    private void OnOverlayCommandRequested(object? sender, OverlayCommandRequestedEventArgs e)
    {
        switch (e.Command)
        {
            case OverlayCommand.Next:
                if (_sessionController.MoveNext() && _sessionController.CurrentSession is not null)
                {
                    Application.Current.Dispatcher.BeginInvoke(() => _overlayController.UpdateSession(_sessionController.CurrentSession));
                    _debugWindowController.SetLastAction($"Selection moved: {_sessionController.CurrentSession.SelectedIndex}");
                }
                break;
            case OverlayCommand.Previous:
                if (_sessionController.MovePrevious() && _sessionController.CurrentSession is not null)
                {
                    Application.Current.Dispatcher.BeginInvoke(() => _overlayController.UpdateSession(_sessionController.CurrentSession));
                    _debugWindowController.SetLastAction($"Selection moved: {_sessionController.CurrentSession.SelectedIndex}");
                }
                break;
            case OverlayCommand.Commit:
                CommitSession();
                break;
            case OverlayCommand.Cancel:
                CancelSession();
                break;
        }
    }

    private void OnOverlayDeactivated(object? sender, OverlayDeactivatedEventArgs e)
    {
        if (_sessionController.CurrentSession is null)
        {
            return;
        }

        if (!_extensionRegistry.OverlayDismissBehaviors.Any(behavior => behavior.CancelOnOverlayLostFocus))
        {
            return;
        }

        _logger.Info("Overlay lost focus; canceling session through overlay dismiss behavior.");
        _debugWindowController.SetLastAction("Overlay lost focus -> canceled");
        CancelSession();
    }

    private void CommitSession()
    {
        var result = _sessionController.CommitCurrentSession();
        Application.Current.Dispatcher.BeginInvoke(() => _overlayController.HideOverlay());

        if (!result.Success || result.SelectedWindow is null)
        {
            _logger.Info($"Commit ignored: {result.Reason}");
            _debugWindowController.SetLastAction($"Commit ignored: {result.Reason}");
            return;
        }

        var activated = _windowActivationService.TryActivateWindow(result.SelectedWindow.Handle);
        _logger.Info(
            $"Commit session {result.SessionId}: title='{result.SelectedWindow.Title}', process='{result.SelectedWindow.ProcessName}', activated={activated}.");
        _debugWindowController.SetSessionStatus("Idle");
        _debugWindowController.SetLastAction($"Committed: {result.SelectedWindow.Title}");
    }

    private void CancelSession()
    {
        var originalForegroundWindowHandle = _sessionController.CurrentSession?.OriginalForegroundWindowHandle ?? 0;
        if (_sessionController.CancelCurrentSession())
        {
            Application.Current.Dispatcher.BeginInvoke(() => _overlayController.HideOverlay());
            if (originalForegroundWindowHandle != 0)
            {
                _windowActivationService.TryActivateWindow(originalForegroundWindowHandle);
            }

            _logger.Info("Session canceled.");
            _debugWindowController.SetSessionStatus("Idle");
            _debugWindowController.SetLastAction("Canceled");
        }
    }

    private IReadOnlyList<IWindowFilterRule> ResolveFilters()
    {
        return ResolveFilters(new BuiltInWindowFilter(_hostProcessId));
    }

    private IReadOnlyList<IWindowFilterRule> ResolveFilters(BuiltInWindowFilter builtInFilter)
    {
        var filters = new List<IWindowFilterRule>
        {
            builtInFilter,
        };

        foreach (var extensionFilter in _extensionRegistry.WindowFilters)
        {
            filters.Add(new ExtensionWindowFilterAdapter(extensionFilter));
        }

        return filters;
    }

    private void LogBuiltInFilterSummary(
        IReadOnlyList<WindowInfo> rawWindows,
        BuiltInWindowFilter builtInFilter)
    {
        var reasonCounts = rawWindows
            .Select(window => builtInFilter.GetExclusionReason(window))
            .Where(reason => reason is not null)
            .GroupBy(reason => reason!, StringComparer.Ordinal)
            .OrderByDescending(group => group.Count())
            .ThenBy(group => group.Key, StringComparer.Ordinal)
            .ToArray();

        if (reasonCounts.Length == 0)
        {
            _logger.Debug("Built-in filter summary: no windows excluded by built-in rules.");
            return;
        }

        var summary = string.Join(", ", reasonCounts.Select(group => $"{group.Key}={group.Count()}"));
        _logger.Debug($"Built-in filter summary: {summary}");
    }

    private IWindowSortRule ResolveSortRule()
    {
        if (!string.IsNullOrWhiteSpace(PreferredSortStrategyId))
        {
            var preferred = _extensionRegistry.SortStrategies
                .FirstOrDefault(strategy => string.Equals(strategy.Id, PreferredSortStrategyId, StringComparison.OrdinalIgnoreCase));

            if (preferred is not null)
            {
                _logger.Info($"Using preferred external sort strategy '{preferred.Id}'.");
                return new ExtensionSortStrategyAdapter(preferred);
            }
        }

        _logger.Debug($"Using built-in default sort strategy '{_defaultSortStrategy.Id}'.");
        return _defaultSortStrategy;
    }
}
