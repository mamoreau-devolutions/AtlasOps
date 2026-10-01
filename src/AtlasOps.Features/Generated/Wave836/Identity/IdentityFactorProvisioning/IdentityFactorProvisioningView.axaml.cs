namespace AtlasOps.Features.Identity.IdentityFactorProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityFactorProvisioningView : UserControl
{
    public IdentityFactorProvisioningView()
    {
        this.DataContext = new IdentityFactorProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityFactorProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}