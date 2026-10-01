namespace AtlasOps.Features.Cloud.CloudNetworkMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudNetworkMonitoringView : UserControl
{
    public CloudNetworkMonitoringView()
    {
        this.DataContext = new CloudNetworkMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudNetworkMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}