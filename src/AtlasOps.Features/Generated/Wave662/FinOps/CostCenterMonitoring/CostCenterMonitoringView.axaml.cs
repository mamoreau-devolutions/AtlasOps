namespace AtlasOps.Features.FinOps.CostCenterMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CostCenterMonitoringView : UserControl
{
    public CostCenterMonitoringView()
    {
        this.DataContext = new CostCenterMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CostCenterMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}