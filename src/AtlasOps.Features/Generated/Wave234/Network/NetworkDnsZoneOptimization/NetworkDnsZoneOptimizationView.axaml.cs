namespace AtlasOps.Features.Network.NetworkDnsZoneOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkDnsZoneOptimizationView : UserControl
{
    public NetworkDnsZoneOptimizationView()
    {
        this.DataContext = new NetworkDnsZoneOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkDnsZoneOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}