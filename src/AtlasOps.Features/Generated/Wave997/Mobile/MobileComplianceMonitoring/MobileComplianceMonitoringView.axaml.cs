namespace AtlasOps.Features.Mobile.MobileComplianceMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileComplianceMonitoringView : UserControl
{
    public MobileComplianceMonitoringView()
    {
        this.DataContext = new MobileComplianceMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileComplianceMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}