namespace AtlasOps.Features.Security.SecurityScanMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityScanMonitoringView : UserControl
{
    public SecurityScanMonitoringView()
    {
        this.DataContext = new SecurityScanMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityScanMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}