namespace AtlasOps.Features.Architecture.ArchitectureReviewMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureReviewMonitoringView : UserControl
{
    public ArchitectureReviewMonitoringView()
    {
        this.DataContext = new ArchitectureReviewMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureReviewMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}