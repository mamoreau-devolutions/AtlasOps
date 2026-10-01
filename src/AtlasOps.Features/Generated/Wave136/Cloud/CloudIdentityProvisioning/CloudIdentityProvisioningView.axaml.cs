namespace AtlasOps.Features.Cloud.CloudIdentityProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CloudIdentityProvisioningView : UserControl
{
    public CloudIdentityProvisioningView()
    {
        this.DataContext = new CloudIdentityProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CloudIdentityProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}