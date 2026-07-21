using System.IO;
using System.Windows;
using CoverflowAltTab.Core.Services;
using CoverflowAltTab.Extensibility;
using CoverflowAltTab.Host.Debug;
using CoverflowAltTab.Host.Infrastructure;
using CoverflowAltTab.Platform;
using CoverflowAltTab.UI;
using CoverflowAltTab.UI.Rendering;

namespace CoverflowAltTab.Host;

public partial class App : Application
{
    private IHotkeyService? _hotkeyService;
    private IKeyboardMonitorService? _keyboardMonitorService;
    private SwitcherApplication? _switcherApplication;
    private DebugWindowController? _debugWindowController;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var logger = new FileDebugLogger(AppContext.BaseDirectory);
        DispatcherUnhandledException += (_, args) =>
        {
            logger.Error("Unhandled dispatcher exception.", args.Exception);
            args.Handled = true;
        };
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            logger.Error("Unhandled app domain exception.", args.ExceptionObject as Exception);
        };

        var registry = new ExtensionRegistry();
        var loader = new ExtensionLoader(registry, logger);
        var loadResults = loader.LoadFromDirectory(Path.Combine(AppContext.BaseDirectory, "extensions"));

        _debugWindowController = new DebugWindowController();
        MainWindow = _debugWindowController.Window;
        _debugWindowController.Show();
        _debugWindowController.SetAppStatus("Running");
        _debugWindowController.SetHotkeyStatus("Ctrl + Shift + Space");
        _debugWindowController.SetSessionStatus("Idle");
        _debugWindowController.SetLastAction("Startup complete");
        _debugWindowController.SetExtensions(loadResults);

        _hotkeyService = new HotkeyService();
        _keyboardMonitorService = new LowLevelKeyboardMonitorService();
        _switcherApplication = new SwitcherApplication(
            new SessionController(new WindowPipeline()),
            new WindowEnumerationService(new Win32WindowInspector()),
            new ForegroundWindowService(),
            new WindowActivationService(),
            new OverlayController(new ListOverlayRenderer()),
            _hotkeyService,
            _keyboardMonitorService,
            registry,
            logger,
            Environment.ProcessId,
            _debugWindowController);

        logger.Info("CoverflowAltTab host startup complete.");
        _switcherApplication.Start();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _switcherApplication?.Dispose();
        _keyboardMonitorService?.Dispose();
        _hotkeyService?.Dispose();
        base.OnExit(e);
    }
}
