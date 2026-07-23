using System.IO;
using System.Windows;
using CoverflowAltTab.Core.Services;
using CoverflowAltTab.Extensibility;
using CoverflowAltTab.Host.Debug;
using CoverflowAltTab.Host.Infrastructure;
using CoverflowAltTab.Platform;
using CoverflowAltTab.UI;
using CoverflowAltTab.UI.Configuration;
using CoverflowAltTab.UI.Motion;
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

        var settings = SettingsService.Load();
        AnimationStrategyRegistry.SetCurrent(settings.AnimationId);

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

        var overlayController = new OverlayController(OverlayRendererRegistry.Resolve(settings.RendererId));

        _debugWindowController.InitializeOverlaySettings(
            OverlayRendererRegistry.AvailableIds,
            AnimationStrategyRegistry.AvailableIds,
            settings.RendererId,
            settings.AnimationId);
        _debugWindowController.RendererSelected += (_, rendererId) =>
        {
            settings = settings with { RendererId = rendererId };
            SettingsService.Save(settings);
            overlayController.SetRenderer(OverlayRendererRegistry.Resolve(rendererId));
            logger.Info($"Overlay renderer switched to '{rendererId}' from debug window.");
        };
        _debugWindowController.AnimationSelected += (_, animationId) =>
        {
            settings = settings with { AnimationId = animationId };
            SettingsService.Save(settings);
            AnimationStrategyRegistry.SetCurrent(animationId);
            logger.Info($"Overlay animation switched to '{animationId}' from debug window.");
        };

        _hotkeyService = new HotkeyService();
        _keyboardMonitorService = new LowLevelKeyboardMonitorService();
        _switcherApplication = new SwitcherApplication(
            new SessionController(new WindowPipeline()),
            new WindowEnumerationService(new Win32WindowInspector()),
            new ForegroundWindowService(),
            new WindowActivationService(),
            new DwmWindowThumbnailService(),
            overlayController,
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
