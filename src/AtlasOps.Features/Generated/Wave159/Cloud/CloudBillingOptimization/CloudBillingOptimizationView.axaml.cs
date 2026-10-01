namespace AtlasOps.Features.Cloud.CloudBillingOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudBillingOptimizationView : UserControl
{
    public CloudBillingOptimizationView()
    {
        this.DataContext = new CloudBillingOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudBillingOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}