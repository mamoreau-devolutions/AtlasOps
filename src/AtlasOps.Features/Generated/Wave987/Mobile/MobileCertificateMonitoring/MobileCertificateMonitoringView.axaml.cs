namespace AtlasOps.Features.Mobile.MobileCertificateMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileCertificateMonitoringView : UserControl
{
    public MobileCertificateMonitoringView()
    {
        this.DataContext = new MobileCertificateMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileCertificateMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}