namespace AtlasOps.Features.Identity.IdentitySessionProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentitySessionProvisioningView : UserControl
{
    public IdentitySessionProvisioningView()
    {
        this.DataContext = new IdentitySessionProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentitySessionProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}