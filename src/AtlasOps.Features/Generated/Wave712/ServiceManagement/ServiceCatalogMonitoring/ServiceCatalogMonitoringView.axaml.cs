namespace AtlasOps.Features.ServiceManagement.ServiceCatalogMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ServiceCatalogMonitoringView : UserControl
{
    public ServiceCatalogMonitoringView()
    {
        this.DataContext = new ServiceCatalogMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ServiceCatalogMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}