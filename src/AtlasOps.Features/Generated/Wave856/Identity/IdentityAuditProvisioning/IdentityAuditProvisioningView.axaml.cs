namespace AtlasOps.Features.Identity.IdentityAuditProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityAuditProvisioningView : UserControl
{
    public IdentityAuditProvisioningView()
    {
        this.DataContext = new IdentityAuditProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityAuditProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}