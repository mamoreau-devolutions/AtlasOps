namespace AtlasOps.Features.Network.NetworkSegmentOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkSegmentOptimizationView : UserControl
{
    public NetworkSegmentOptimizationView()
    {
        this.DataContext = new NetworkSegmentOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkSegmentOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}