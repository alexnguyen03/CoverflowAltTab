using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CoverflowAltTab.Platform;

public sealed class LowLevelKeyboardMonitorService : IKeyboardMonitorService
{
    private Win32.LowLevelKeyboardProc? _hookProc;
    private nint _hookHandle;

    public event EventHandler<KeyEventRecord>? KeyEventReceived;

    public void Start()
    {
        if (_hookHandle != 0)
        {
            return;
        }

        _hookProc = HookCallback;
        using var process = Process.GetCurrentProcess();
        using var module = process.MainModule;
        var moduleHandle = module is null ? 0 : Win32.GetModuleHandle(module.ModuleName);
        _hookHandle = Win32.SetWindowsHookEx(Win32.WH_KEYBOARD_LL, _hookProc, moduleHandle, 0);

        if (_hookHandle == 0)
        {
            throw new InvalidOperationException("Failed to install low-level keyboard hook.");
        }
    }

    public void Stop()
    {
        if (_hookHandle == 0)
        {
            return;
        }

        Win32.UnhookWindowsHookEx(_hookHandle);
        _hookHandle = 0;
        _hookProc = null;
    }

    public void Dispose()
    {
        Stop();
    }

    private nint HookCallback(int code, nint wParam, nint lParam)
    {
        if (code >= 0)
        {
            var message = wParam.ToInt32();
            if (TryMapMessage(message, out var kind))
            {
                var keyboardData = Marshal.PtrToStructure<Win32.KBDLLHOOKSTRUCT>(lParam);
                var record = new KeyEventRecord(
                    kind,
                    unchecked((int)keyboardData.vkCode),
                    Enum.GetName(typeof(System.Windows.Input.Key), System.Windows.Input.KeyInterop.KeyFromVirtualKey(unchecked((int)keyboardData.vkCode)))
                        ?? $"VK_{keyboardData.vkCode:X2}",
                    IsKeyPressed(Win32.VK_CONTROL),
                    IsKeyPressed(Win32.VK_MENU),
                    IsKeyPressed(Win32.VK_SHIFT),
                    DateTimeOffset.Now);

                KeyEventReceived?.Invoke(this, record);
            }
        }

        return Win32.CallNextHookEx(_hookHandle, code, wParam, lParam);
    }

    private static bool TryMapMessage(int message, out KeyEventKind kind)
    {
        switch (message)
        {
            case Win32.WM_KEYDOWN:
                kind = KeyEventKind.KeyDown;
                return true;
            case Win32.WM_KEYUP:
                kind = KeyEventKind.KeyUp;
                return true;
            case Win32.WM_SYSKEYDOWN:
                kind = KeyEventKind.SystemKeyDown;
                return true;
            case Win32.WM_SYSKEYUP:
                kind = KeyEventKind.SystemKeyUp;
                return true;
            default:
                kind = default;
                return false;
        }
    }

    private static bool IsKeyPressed(int virtualKey)
    {
        return (Win32.GetKeyState(virtualKey) & 0x8000) != 0;
    }
}
