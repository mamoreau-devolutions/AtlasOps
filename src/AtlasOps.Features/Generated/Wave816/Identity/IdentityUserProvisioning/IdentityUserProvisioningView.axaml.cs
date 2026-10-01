namespace AtlasOps.Features.Identity.IdentityUserProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityUserProvisioningView : UserControl
{
    public IdentityUserProvisioningView()
    {
        this.DataContext = new IdentityUserProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityUserProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}