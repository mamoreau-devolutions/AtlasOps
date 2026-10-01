namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MaintenanceWindowMonitoringView : UserControl
{
    public MaintenanceWindowMonitoringView()
    {
        this.DataContext = new MaintenanceWindowMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MaintenanceWindowMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}