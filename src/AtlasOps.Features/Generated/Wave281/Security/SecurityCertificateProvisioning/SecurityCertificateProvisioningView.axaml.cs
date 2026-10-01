namespace AtlasOps.Features.Security.SecurityCertificateProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityCertificateProvisioningView : UserControl
{
    public SecurityCertificateProvisioningView()
    {
        this.DataContext = new SecurityCertificateProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityCertificateProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}