namespace AtlasOps.Features.Identity.IdentityLifecycleProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityLifecycleProvisioningView : UserControl
{
    public IdentityLifecycleProvisioningView()
    {
        this.DataContext = new IdentityLifecycleProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityLifecycleProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}