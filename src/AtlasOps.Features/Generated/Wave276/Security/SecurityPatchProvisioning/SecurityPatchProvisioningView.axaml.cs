namespace AtlasOps.Features.Security.SecurityPatchProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityPatchProvisioningView : UserControl
{
    public SecurityPatchProvisioningView()
    {
        this.DataContext = new SecurityPatchProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityPatchProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}