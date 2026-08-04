using System.Windows;

namespace ArenaDesk.Player;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        try
        {
            var options = PlayerScreenOptions.Load();
            MainWindow = new MainWindow(options);
            MainWindow.Show();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"ArenaDesk Player Screen sazlamasy ýalňyş:\n{exception.Message}",
                "ArenaDesk Player Screen",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
        }
    }
}
