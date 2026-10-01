namespace AtlasOps.Features.Architecture.ArchitectureComponentMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureComponentMonitoringView : UserControl
{
    public ArchitectureComponentMonitoringView()
    {
        this.DataContext = new ArchitectureComponentMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureComponentMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}