namespace AtlasOps.Features.Cloud.CloudFunctionOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudFunctionOptimizationView : UserControl
{
    public CloudFunctionOptimizationView()
    {
        this.DataContext = new CloudFunctionOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudFunctionOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}