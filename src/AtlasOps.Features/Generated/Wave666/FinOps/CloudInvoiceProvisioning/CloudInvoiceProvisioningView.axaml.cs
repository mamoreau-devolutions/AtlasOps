namespace AtlasOps.Features.FinOps.CloudInvoiceProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudInvoiceProvisioningView : UserControl
{
    public CloudInvoiceProvisioningView()
    {
        this.DataContext = new CloudInvoiceProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudInvoiceProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}