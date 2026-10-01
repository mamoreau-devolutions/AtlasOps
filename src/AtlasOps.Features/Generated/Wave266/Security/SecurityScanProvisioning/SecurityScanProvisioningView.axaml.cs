namespace AtlasOps.Features.Security.SecurityScanProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityScanProvisioningView : UserControl
{
    public SecurityScanProvisioningView()
    {
        this.DataContext = new SecurityScanProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityScanProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}