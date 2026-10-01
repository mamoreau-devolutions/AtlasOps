namespace AtlasOps.Features.Delivery.ReleasePipelineMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleasePipelineMonitoringView : UserControl
{
    public ReleasePipelineMonitoringView()
    {
        this.DataContext = new ReleasePipelineMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleasePipelineMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}