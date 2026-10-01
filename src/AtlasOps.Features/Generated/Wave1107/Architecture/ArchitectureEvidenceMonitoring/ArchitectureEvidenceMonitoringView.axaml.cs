namespace AtlasOps.Features.Architecture.ArchitectureEvidenceMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureEvidenceMonitoringView : UserControl
{
    public ArchitectureEvidenceMonitoringView()
    {
        this.DataContext = new ArchitectureEvidenceMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureEvidenceMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}