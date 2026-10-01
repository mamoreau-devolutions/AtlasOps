namespace AtlasOps.Features.Connections.CertificateTrust;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CertificateTrustView : UserControl
{
    public CertificateTrustView()
    {
        this.DataContext = new CertificateTrustViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CertificateTrustViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}