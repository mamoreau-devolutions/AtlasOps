namespace AtlasOps.Features.Cloud.CloudFunctionProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudFunctionProvisioningView : UserControl
{
    public CloudFunctionProvisioningView()
    {
        this.DataContext = new CloudFunctionProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudFunctionProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}