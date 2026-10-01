namespace AtlasOps.Features.Cloud.CloudNetworkOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudNetworkOptimizationView : UserControl
{
    public CloudNetworkOptimizationView()
    {
        this.DataContext = new CloudNetworkOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudNetworkOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}