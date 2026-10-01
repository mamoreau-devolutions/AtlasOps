namespace AtlasOps.Features.FinOps.CloudInvoiceOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudInvoiceOptimizationView : UserControl
{
    public CloudInvoiceOptimizationView()
    {
        this.DataContext = new CloudInvoiceOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudInvoiceOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}