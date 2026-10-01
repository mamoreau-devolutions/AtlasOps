namespace AtlasOps.Features.Cloud.CloudBillingProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudBillingProvisioningView : UserControl
{
    public CloudBillingProvisioningView()
    {
        this.DataContext = new CloudBillingProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudBillingProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}