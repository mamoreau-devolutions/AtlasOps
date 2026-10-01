namespace AtlasOps.Features.Identity.IdentityClaimProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityClaimProvisioningView : UserControl
{
    public IdentityClaimProvisioningView()
    {
        this.DataContext = new IdentityClaimProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityClaimProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}