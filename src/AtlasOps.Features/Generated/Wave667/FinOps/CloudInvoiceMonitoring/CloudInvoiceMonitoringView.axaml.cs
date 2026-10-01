namespace AtlasOps.Features.FinOps.CloudInvoiceMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudInvoiceMonitoringView : UserControl
{
    public CloudInvoiceMonitoringView()
    {
        this.DataContext = new CloudInvoiceMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudInvoiceMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}