namespace AtlasOps.Features.Security.SecurityKeyProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityKeyProvisioningView : UserControl
{
    public SecurityKeyProvisioningView()
    {
        this.DataContext = new SecurityKeyProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityKeyProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}