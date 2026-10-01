namespace AtlasOps.Features.Identity.IdentityProviderProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityProviderProvisioningView : UserControl
{
    public IdentityProviderProvisioningView()
    {
        this.DataContext = new IdentityProviderProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityProviderProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}