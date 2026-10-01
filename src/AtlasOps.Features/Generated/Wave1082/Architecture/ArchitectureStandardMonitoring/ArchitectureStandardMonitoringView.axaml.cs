namespace AtlasOps.Features.Architecture.ArchitectureStandardMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureStandardMonitoringView : UserControl
{
    public ArchitectureStandardMonitoringView()
    {
        this.DataContext = new ArchitectureStandardMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureStandardMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}