namespace AtlasOps.Features.Security.SecurityIdentityProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityIdentityProvisioningView : UserControl
{
    public SecurityIdentityProvisioningView()
    {
        this.DataContext = new SecurityIdentityProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityIdentityProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}