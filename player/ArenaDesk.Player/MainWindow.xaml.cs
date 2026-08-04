using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ArenaDesk.Contracts;

namespace ArenaDesk.Player;

public partial class MainWindow : Window
{
    private readonly CancellationTokenSource _stopping = new();
    private readonly PlayerChannelClient _channel;
    private readonly DispatcherTimer _timer;
    private DateTimeOffset? _endsAtUtc;

    public MainWindow(PlayerScreenOptions options)
    {
        InitializeComponent();
        _channel = new PlayerChannelClient(options);
        _channel.StateReceived += state => Dispatcher.InvokeAsync(() => ApplyState(state));
        _channel.ConnectionChanged += connected => Dispatcher.InvokeAsync(() => ShowConnection(connected));
        _timer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, OnTimerTick, Dispatcher);
        Loaded += (_, _) =>
        {
            ShowLocked("Sessiya açylmagyna garaşylýar");
            _ = _channel.RunAsync(_stopping.Token);
            _timer.Start();
        };
    }

    private void ApplyState(PlayerScreenState state)
    {
        ComputerCodeText.Text = state.ComputerCode;
        ActiveComputerCodeText.Text = state.ComputerCode;
        if (state.Mode == PlayerScreenProtocol.ActiveMode && state.EndsAtUtc > DateTimeOffset.UtcNow)
        {
            ShowActive(state.EndsAtUtc.Value);
            return;
        }

        ShowLocked(state.Message);
    }

    private void ShowLocked(string message)
    {
        _endsAtUtc = null;
        LockedMessageText.Text = message;
        SessionPanel.Visibility = Visibility.Collapsed;
        LockedPanel.Visibility = Visibility.Visible;
        WindowState = WindowState.Normal;
        Left = 0;
        Top = 0;
        Width = SystemParameters.PrimaryScreenWidth;
        Height = SystemParameters.PrimaryScreenHeight;
        Topmost = true;
        Activate();
    }

    private void ShowActive(DateTimeOffset endsAtUtc)
    {
        _endsAtUtc = endsAtUtc;
        LockedPanel.Visibility = Visibility.Collapsed;
        SessionPanel.Visibility = Visibility.Visible;
        WindowState = WindowState.Normal;
        Width = 330;
        Height = 108;
        Left = SystemParameters.WorkArea.Right - Width - 22;
        Top = SystemParameters.WorkArea.Top + 22;
        EndTimeText.Text = endsAtUtc.ToLocalTime().ToString("HH:mm");
        UpdateRemainingTime();
    }

    private void OnTimerTick(object? sender, EventArgs e) => UpdateRemainingTime();

    private void UpdateRemainingTime()
    {
        if (_endsAtUtc is not DateTimeOffset endsAtUtc)
        {
            return;
        }

        var remaining = endsAtUtc - DateTimeOffset.UtcNow;
        if (remaining <= TimeSpan.Zero)
        {
            ShowLocked("Wagt tamamlandy");
            return;
        }

        RemainingTimeText.Text = remaining.TotalHours >= 1
            ? $"{(int)remaining.TotalHours:00}:{remaining.Minutes:00}:{remaining.Seconds:00}"
            : $"{remaining.Minutes:00}:{remaining.Seconds:00}";
        if (remaining <= TimeSpan.FromMinutes(5))
        {
            SessionStatusText.Text = "5 MINUTDAN AZ GALDY";
            SessionStatusText.Foreground = Brush("#FCA5A5");
            SessionStatusDot.Fill = Brush("#F87171");
        }
        else if (remaining <= TimeSpan.FromMinutes(10))
        {
            SessionStatusText.Text = "10 MINUTDAN AZ GALDY";
            SessionStatusText.Foreground = Brush("#FCD34D");
            SessionStatusDot.Fill = Brush("#F59E0B");
        }
        else
        {
            SessionStatusText.Text = "SESSIÝA DOWAM EDÝÄR";
            SessionStatusText.Foreground = Brush("#6EE7B7");
            SessionStatusDot.Fill = Brush("#34D399");
        }
    }

    private void ShowConnection(bool connected)
    {
        ConnectionText.Text = connected ? "Agent bilen baglanyşyk bar" : "Agent bilen baglanyşyk ýok";
        ConnectionDot.Fill = Brush(connected ? "#34D399" : "#F59E0B");
    }

    private static SolidColorBrush Brush(string color) =>
        new((Color)ColorConverter.ConvertFromString(color)!);

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F4 && Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
        {
            e.Handled = true;
        }
    }

    private void OnStateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            WindowState = WindowState.Normal;
        }
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        e.Cancel = true;
    }
}
