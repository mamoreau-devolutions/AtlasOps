namespace AtlasOps.Features.FinOps.CostAnomalyMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CostAnomalyMonitoringView : UserControl
{
    public CostAnomalyMonitoringView()
    {
        this.DataContext = new CostAnomalyMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CostAnomalyMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}