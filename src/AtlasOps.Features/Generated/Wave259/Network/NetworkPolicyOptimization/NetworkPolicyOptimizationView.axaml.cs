namespace AtlasOps.Features.Network.NetworkPolicyOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkPolicyOptimizationView : UserControl
{
    public NetworkPolicyOptimizationView()
    {
        this.DataContext = new NetworkPolicyOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkPolicyOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}