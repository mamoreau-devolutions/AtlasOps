namespace AtlasOps.Features.Compute.ComputeScaleSetMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeScaleSetMonitoringView : UserControl
{
    public ComputeScaleSetMonitoringView()
    {
        this.DataContext = new ComputeScaleSetMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeScaleSetMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}