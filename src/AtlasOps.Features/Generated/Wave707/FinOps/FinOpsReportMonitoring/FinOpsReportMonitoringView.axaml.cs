namespace AtlasOps.Features.FinOps.FinOpsReportMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class FinOpsReportMonitoringView : UserControl
{
    public FinOpsReportMonitoringView()
    {
        this.DataContext = new FinOpsReportMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is FinOpsReportMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}