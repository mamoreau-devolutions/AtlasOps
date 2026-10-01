namespace AtlasOps.Features.Identity.IdentityGroupProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityGroupProvisioningView : UserControl
{
    public IdentityGroupProvisioningView()
    {
        this.DataContext = new IdentityGroupProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityGroupProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}