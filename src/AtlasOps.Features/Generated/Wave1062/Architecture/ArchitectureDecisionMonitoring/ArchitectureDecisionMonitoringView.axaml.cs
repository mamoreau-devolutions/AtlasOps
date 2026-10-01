namespace AtlasOps.Features.Architecture.ArchitectureDecisionMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureDecisionMonitoringView : UserControl
{
    public ArchitectureDecisionMonitoringView()
    {
        this.DataContext = new ArchitectureDecisionMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureDecisionMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}