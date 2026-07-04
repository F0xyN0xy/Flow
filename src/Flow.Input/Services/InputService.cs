using Flow.Core.Services;
using Flow.Input.Native;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Flow.Input.Services;

/// <summary>
/// Registers a system-wide hotkey (configured via <c>AppSettings.Hotkey</c>) that toggles
/// the overlay, even while Rocket League has input focus. Implemented with a hidden
/// message-only window and Win32 RegisterHotKey, so no external input-hook library or
/// process injection is required. Windows-only.
/// </summary>
public sealed class InputService : IInputService, IHostedService, IDisposable
{
    private const uint WM_HOTKEY = 0x0312;
    private const uint WM_APP_SHUTDOWN = 0x8001; // custom message used to unwind the message loop
    private const uint MOD_NOREPEAT = 0x4000;
    private const int HotkeyId = 1;

    private readonly ILogger<InputService> _logger;
    private readonly ISettingsService _settingsService;

    private Thread? _messageLoopThread;
    private readonly ManualResetEventSlim _windowReady = new(false);
    private IntPtr _windowHandle;
    private WndProcDelegate? _wndProcDelegate; // rooted so the GC never collects the callback
    private uint _registeredVirtualKey;

    /// <inheritdoc />
    public event EventHandler? ToggleOverlayRequested;

    /// <summary>
    /// Initializes a new instance of the <see cref="InputService"/> class.
    /// </summary>
    public InputService(ILogger<InputService> logger, ISettingsService settingsService)
    {
        _logger = logger;
        _settingsService = settingsService;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (!OperatingSystem.IsWindows())
        {
            _logger.LogWarning("Global hotkeys require Windows; the input service is inactive on this platform.");
            return Task.CompletedTask;
        }

        _logger.LogInformation("Input service starting...");
        _settingsService.Current.PropertyChanged += OnSettingsPropertyChanged;

        _messageLoopThread = new Thread(RunMessageLoop)
        {
            IsBackground = true,
            Name = "Flow.Input.HotkeyLoop",
        };
        _messageLoopThread.Start();

        // Give the loop thread a moment to create its window before returning, so a
        // hotkey pressed immediately after startup isn't silently dropped.
        _windowReady.Wait(TimeSpan.FromSeconds(2));

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Input service stopping...");
        _settingsService.Current.PropertyChanged -= OnSettingsPropertyChanged;

        if (_windowHandle != IntPtr.Zero)
        {
            NativeMethods.PostMessage(_windowHandle, WM_APP_SHUTDOWN, IntPtr.Zero, IntPtr.Zero);
        }

        _messageLoopThread?.Join(TimeSpan.FromSeconds(2));
        return Task.CompletedTask;
    }

    private void OnSettingsPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Core.Models.AppSettings.Hotkey))
        {
            RegisterConfiguredHotkey();
        }
    }

    private void RunMessageLoop()
    {
        _windowHandle = NativeMethods.CreateMessageWindow(out _wndProcDelegate, HandleWindowMessage);
        RegisterConfiguredHotkey();
        _windowReady.Set();

        while (NativeMethods.GetMessage(out var msg, IntPtr.Zero, 0, 0) > 0)
        {
            NativeMethods.TranslateMessage(ref msg);
            NativeMethods.DispatchMessage(ref msg);
        }

        if (_registeredVirtualKey != 0)
        {
            NativeMethods.UnregisterHotKey(_windowHandle, HotkeyId);
        }

        NativeMethods.DestroyWindow(_windowHandle);
    }

    private IntPtr HandleWindowMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        switch (msg)
        {
            case WM_HOTKEY when wParam.ToInt32() == HotkeyId:
                ToggleOverlayRequested?.Invoke(this, EventArgs.Empty);
                return IntPtr.Zero;

            case WM_APP_SHUTDOWN:
                NativeMethods.PostQuitMessage(0);
                return IntPtr.Zero;

            default:
                return NativeMethods.DefWindowProc(hWnd, msg, wParam, lParam);
        }
    }

    private void RegisterConfiguredHotkey()
    {
        if (_windowHandle == IntPtr.Zero)
        {
            return;
        }

        if (_registeredVirtualKey != 0)
        {
            NativeMethods.UnregisterHotKey(_windowHandle, HotkeyId);
            _registeredVirtualKey = 0;
        }

        var hotkeyName = _settingsService.Current.Hotkey;
        if (!VirtualKeyMap.TryGetVirtualKey(hotkeyName, out var virtualKey))
        {
            _logger.LogWarning("Unrecognized hotkey '{Hotkey}'; overlay toggle hotkey is disabled.", hotkeyName);
            return;
        }

        if (NativeMethods.RegisterHotKey(_windowHandle, HotkeyId, MOD_NOREPEAT, virtualKey))
        {
            _registeredVirtualKey = virtualKey;
            _logger.LogInformation("Registered global hotkey '{Hotkey}' to toggle the overlay.", hotkeyName);
        }
        else
        {
            _logger.LogWarning(
                "Failed to register global hotkey '{Hotkey}'. It may already be bound by another application (including Rocket League itself).",
                hotkeyName);
        }
    }

    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        return StartAsync(cancellationToken);
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        return StopAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _windowReady.Dispose();
    }
}
