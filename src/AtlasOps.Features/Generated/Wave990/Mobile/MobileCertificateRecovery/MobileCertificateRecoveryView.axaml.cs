namespace AtlasOps.Features.Mobile.MobileCertificateRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileCertificateRecoveryView : UserControl
{
    public MobileCertificateRecoveryView()
    {
        this.DataContext = new MobileCertificateRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileCertificateRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}