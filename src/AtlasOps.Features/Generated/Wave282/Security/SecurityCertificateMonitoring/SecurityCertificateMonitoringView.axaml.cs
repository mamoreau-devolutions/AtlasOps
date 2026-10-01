namespace AtlasOps.Features.Security.SecurityCertificateMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityCertificateMonitoringView : UserControl
{
    public SecurityCertificateMonitoringView()
    {
        this.DataContext = new SecurityCertificateMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityCertificateMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}