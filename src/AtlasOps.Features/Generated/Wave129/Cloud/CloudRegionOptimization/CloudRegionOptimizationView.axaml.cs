namespace AtlasOps.Features.Cloud.CloudRegionOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudRegionOptimizationView : UserControl
{
    public CloudRegionOptimizationView()
    {
        this.DataContext = new CloudRegionOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudRegionOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}