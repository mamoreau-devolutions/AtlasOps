namespace AtlasOps.Features.Network.NetworkVpnOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkVpnOptimizationView : UserControl
{
    public NetworkVpnOptimizationView()
    {
        this.DataContext = new NetworkVpnOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkVpnOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}