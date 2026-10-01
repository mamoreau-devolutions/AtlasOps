namespace AtlasOps.Features.Desktop.DesktopLicenseProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopLicenseProvisioningView : UserControl
{
    public DesktopLicenseProvisioningView()
    {
        this.DataContext = new DesktopLicenseProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopLicenseProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}