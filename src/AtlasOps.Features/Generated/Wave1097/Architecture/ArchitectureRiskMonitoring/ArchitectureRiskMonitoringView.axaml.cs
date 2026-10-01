namespace AtlasOps.Features.Architecture.ArchitectureRiskMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureRiskMonitoringView : UserControl
{
    public ArchitectureRiskMonitoringView()
    {
        this.DataContext = new ArchitectureRiskMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureRiskMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}