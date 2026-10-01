namespace AtlasOps.Features.Compute.ComputeScheduleMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeScheduleMonitoringView : UserControl
{
    public ComputeScheduleMonitoringView()
    {
        this.DataContext = new ComputeScheduleMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeScheduleMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}