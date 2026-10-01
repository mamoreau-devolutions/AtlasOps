namespace AtlasOps.Features.Network.NetworkProbeOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkProbeOptimizationView : UserControl
{
    public NetworkProbeOptimizationView()
    {
        this.DataContext = new NetworkProbeOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkProbeOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}