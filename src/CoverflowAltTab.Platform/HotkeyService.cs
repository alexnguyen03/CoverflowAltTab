using System.Windows.Interop;

namespace CoverflowAltTab.Platform;

public sealed class HotkeyService : IHotkeyService
{
    private const int HotkeyId = 0xC0DE;
    private HwndSource? _source;
    private bool _registered;

    public event EventHandler? Triggered;

    public void Start()
    {
        if (_registered)
        {
            return;
        }

        EnsureSource();
        if (!Win32.RegisterHotKey(_source!.Handle, HotkeyId, Win32.MOD_CONTROL | Win32.MOD_SHIFT, (uint)Win32.VK_SPACE))
        {
            throw new InvalidOperationException("Failed to register Ctrl+Shift+Space hotkey.");
        }

        _registered = true;
    }

    public void Stop()
    {
        if (!_registered || _source is null)
        {
            return;
        }

        Win32.UnregisterHotKey(_source.Handle, HotkeyId);
        _registered = false;
    }

    public void Dispose()
    {
        Stop();
        if (_source is not null)
        {
            _source.RemoveHook(WndProc);
            _source.Dispose();
            _source = null;
        }
    }

    private void EnsureSource()
    {
        if (_source is not null)
        {
            return;
        }

        var parameters = new HwndSourceParameters("CoverflowAltTabHotkeySink")
        {
            Width = 0,
            Height = 0,
            WindowStyle = 0,
        };

        _source = new HwndSource(parameters);
        _source.AddHook(WndProc);
    }

    private nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        if (msg == Win32.WM_HOTKEY && wParam.ToInt32() == HotkeyId)
        {
            handled = true;
            Triggered?.Invoke(this, EventArgs.Empty);
        }

        return IntPtr.Zero;
    }
}
