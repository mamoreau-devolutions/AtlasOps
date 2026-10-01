namespace AtlasOps.Features.Desktop.DesktopLicenseMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopLicenseMonitoringView : UserControl
{
    public DesktopLicenseMonitoringView()
    {
        this.DataContext = new DesktopLicenseMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopLicenseMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}