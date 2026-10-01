namespace AtlasOps.Features.Mobile.MobileCertificateGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileCertificateGovernanceView : UserControl
{
    public MobileCertificateGovernanceView()
    {
        this.DataContext = new MobileCertificateGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileCertificateGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}