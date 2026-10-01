namespace AtlasOps.Features.Desktop.DesktopHealthMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopHealthMonitoringView : UserControl
{
    public DesktopHealthMonitoringView()
    {
        this.DataContext = new DesktopHealthMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopHealthMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}