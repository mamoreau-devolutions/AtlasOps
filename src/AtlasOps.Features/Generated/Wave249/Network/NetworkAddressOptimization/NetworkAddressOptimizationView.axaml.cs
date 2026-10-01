namespace AtlasOps.Features.Network.NetworkAddressOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkAddressOptimizationView : UserControl
{
    public NetworkAddressOptimizationView()
    {
        this.DataContext = new NetworkAddressOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkAddressOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}