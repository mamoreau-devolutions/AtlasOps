namespace AtlasOps.App;

using AtlasOps.App.Services;
using AtlasOps.App.ViewModels;
using AtlasOps.App.Views;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            MainWindow window = new();
            window.DataContext = new MainViewModel(new AtlasOpsBootstrapper());
            desktop.MainWindow = window;

            if (desktop.Args?.Contains("--smoke-ui", StringComparer.OrdinalIgnoreCase) == true)
            {
                DispatcherTimer timer = new()
                {
                    Interval = TimeSpan.FromSeconds(2),
                };
                timer.Tick += (_, _) =>
                {
                    timer.Stop();
                    desktop.Shutdown();
                };
                timer.Start();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}