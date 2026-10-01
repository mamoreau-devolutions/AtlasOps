namespace AtlasOps.Features.Identity.IdentityRoleProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityRoleProvisioningView : UserControl
{
    public IdentityRoleProvisioningView()
    {
        this.DataContext = new IdentityRoleProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityRoleProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}