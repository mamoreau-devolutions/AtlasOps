namespace AtlasOps.Features.Delivery.BuildPipelineMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BuildPipelineMonitoringView : UserControl
{
    public BuildPipelineMonitoringView()
    {
        this.DataContext = new BuildPipelineMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BuildPipelineMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}