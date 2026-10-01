namespace AtlasOps.Features.ServiceManagement.ServiceRequestMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceRequestMonitoringView : UserControl
{
    public ServiceRequestMonitoringView()
    {
        this.DataContext = new ServiceRequestMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceRequestMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}