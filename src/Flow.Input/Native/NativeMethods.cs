using System.Runtime.InteropServices;

namespace Flow.Input.Native;

/// <summary>
/// Delegate matching the Win32 WNDPROC signature.
/// </summary>
internal delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

[StructLayout(LayoutKind.Sequential)]
internal struct MSG
{
    public IntPtr hwnd;
    public uint message;
    public IntPtr wParam;
    public IntPtr lParam;
    public uint time;
    public POINT pt;
}

[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
    public int x;
    public int y;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct WNDCLASSEX
{
    public int cbSize;
    public uint style;
    public WndProcDelegate lpfnWndProc;
    public int cbClsExtra;
    public int cbWndExtra;
    public IntPtr hInstance;
    public IntPtr hIcon;
    public IntPtr hCursor;
    public IntPtr hbrBackground;
    public string? lpszMenuName;
    public string lpszClassName;
    public IntPtr hIconSm;
}

/// <summary>
/// Minimal Win32 P/Invoke surface needed to run a hidden message-only window that
/// can receive WM_HOTKEY notifications, without pulling in a WinForms/WPF dependency.
/// </summary>
internal static class NativeMethods
{
    private const int HWND_MESSAGE = -3;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr CreateWindowEx(
        int dwExStyle, string lpClassName, string lpWindowName, int dwStyle,
        int x, int y, int nWidth, int nHeight,
        IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string? lpModuleName);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    public static extern int GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    public static extern bool TranslateMessage(ref MSG lpMsg);

    [DllImport("user32.dll")]
    public static extern IntPtr DispatchMessage(ref MSG lpMsg);

    [DllImport("user32.dll")]
    public static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern void PostQuitMessage(int nExitCode);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool DestroyWindow(IntPtr hWnd);

    /// <summary>
    /// Creates a hidden, message-only window whose sole purpose is to receive WM_HOTKEY
    /// messages on the calling thread. Must be called from the thread that will run the
    /// message loop (<see cref="GetMessage"/>/<see cref="DispatchMessage"/>).
    /// </summary>
    public static IntPtr CreateMessageWindow(out WndProcDelegate wndProc, WndProcDelegate handler)
    {
        wndProc = handler;
        var className = "FlowInputHotkeyWindow_" + Guid.NewGuid().ToString("N");
        var hInstance = GetModuleHandle(null);

        var wndClass = new WNDCLASSEX
        {
            cbSize = Marshal.SizeOf<WNDCLASSEX>(),
            lpfnWndProc = wndProc,
            hInstance = hInstance,
            lpszClassName = className,
        };

        RegisterClassEx(ref wndClass);

        return CreateWindowEx(
            0, className, className, 0,
            0, 0, 0, 0,
            new IntPtr(HWND_MESSAGE), IntPtr.Zero, hInstance, IntPtr.Zero);
    }
}
