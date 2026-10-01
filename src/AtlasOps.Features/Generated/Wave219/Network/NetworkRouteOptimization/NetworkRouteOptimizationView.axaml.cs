namespace AtlasOps.Features.Network.NetworkRouteOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkRouteOptimizationView : UserControl
{
    public NetworkRouteOptimizationView()
    {
        this.DataContext = new NetworkRouteOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkRouteOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}