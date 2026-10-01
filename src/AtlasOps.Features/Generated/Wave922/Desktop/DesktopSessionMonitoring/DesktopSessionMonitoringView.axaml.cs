namespace AtlasOps.Features.Desktop.DesktopSessionMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopSessionMonitoringView : UserControl
{
    public DesktopSessionMonitoringView()
    {
        this.DataContext = new DesktopSessionMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopSessionMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}