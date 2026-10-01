namespace AtlasOps.Features.Network.NetworkPeerOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkPeerOptimizationView : UserControl
{
    public NetworkPeerOptimizationView()
    {
        this.DataContext = new NetworkPeerOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkPeerOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}