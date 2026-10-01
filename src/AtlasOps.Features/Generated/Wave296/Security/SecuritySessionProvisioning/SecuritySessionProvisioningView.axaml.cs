namespace AtlasOps.Features.Security.SecuritySessionProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecuritySessionProvisioningView : UserControl
{
    public SecuritySessionProvisioningView()
    {
        this.DataContext = new SecuritySessionProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecuritySessionProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}