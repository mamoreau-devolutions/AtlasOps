namespace AtlasOps.Features.FinOps.CostAllocationMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CostAllocationMonitoringView : UserControl
{
    public CostAllocationMonitoringView()
    {
        this.DataContext = new CostAllocationMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CostAllocationMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}