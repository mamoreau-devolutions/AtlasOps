namespace AtlasOps.Features.Network.NetworkProbeMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkProbeMonitoringView : UserControl
{
    public NetworkProbeMonitoringView()
    {
        this.DataContext = new NetworkProbeMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkProbeMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}