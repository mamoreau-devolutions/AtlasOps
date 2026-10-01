namespace AtlasOps.Features.Data.DataPipelineMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataPipelineMonitoringView : UserControl
{
    public DataPipelineMonitoringView()
    {
        this.DataContext = new DataPipelineMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataPipelineMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}