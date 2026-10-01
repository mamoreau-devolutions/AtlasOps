namespace AtlasOps.Features.Security.SecurityCertificateRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityCertificateRecoveryView : UserControl
{
    public SecurityCertificateRecoveryView()
    {
        this.DataContext = new SecurityCertificateRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityCertificateRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}