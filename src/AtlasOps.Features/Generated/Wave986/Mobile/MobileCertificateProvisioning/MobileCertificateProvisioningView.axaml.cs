namespace AtlasOps.Features.Mobile.MobileCertificateProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileCertificateProvisioningView : UserControl
{
    public MobileCertificateProvisioningView()
    {
        this.DataContext = new MobileCertificateProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileCertificateProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}