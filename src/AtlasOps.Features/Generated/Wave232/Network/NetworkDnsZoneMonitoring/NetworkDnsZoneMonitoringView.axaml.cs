namespace AtlasOps.Features.Network.NetworkDnsZoneMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkDnsZoneMonitoringView : UserControl
{
    public NetworkDnsZoneMonitoringView()
    {
        this.DataContext = new NetworkDnsZoneMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkDnsZoneMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}