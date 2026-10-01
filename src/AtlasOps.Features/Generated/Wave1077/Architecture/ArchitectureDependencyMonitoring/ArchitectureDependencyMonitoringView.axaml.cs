namespace AtlasOps.Features.Architecture.ArchitectureDependencyMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureDependencyMonitoringView : UserControl
{
    public ArchitectureDependencyMonitoringView()
    {
        this.DataContext = new ArchitectureDependencyMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureDependencyMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}