namespace AtlasOps.Features.Network.NetworkAddressMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkAddressMonitoringView : UserControl
{
    public NetworkAddressMonitoringView()
    {
        this.DataContext = new NetworkAddressMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkAddressMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}