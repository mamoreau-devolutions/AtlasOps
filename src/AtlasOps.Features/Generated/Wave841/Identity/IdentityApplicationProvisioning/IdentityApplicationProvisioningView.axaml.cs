namespace AtlasOps.Features.Identity.IdentityApplicationProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityApplicationProvisioningView : UserControl
{
    public IdentityApplicationProvisioningView()
    {
        this.DataContext = new IdentityApplicationProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityApplicationProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}