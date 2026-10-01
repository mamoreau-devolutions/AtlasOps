namespace AtlasOps.Features.Architecture.ArchitectureRoadmapMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureRoadmapMonitoringView : UserControl
{
    public ArchitectureRoadmapMonitoringView()
    {
        this.DataContext = new ArchitectureRoadmapMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureRoadmapMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}