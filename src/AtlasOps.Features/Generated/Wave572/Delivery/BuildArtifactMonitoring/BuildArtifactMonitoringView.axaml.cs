namespace AtlasOps.Features.Delivery.BuildArtifactMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BuildArtifactMonitoringView : UserControl
{
    public BuildArtifactMonitoringView()
    {
        this.DataContext = new BuildArtifactMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BuildArtifactMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}