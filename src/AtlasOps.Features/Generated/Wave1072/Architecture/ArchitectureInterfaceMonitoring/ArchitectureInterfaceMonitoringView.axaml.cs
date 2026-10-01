namespace AtlasOps.Features.Architecture.ArchitectureInterfaceMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ArchitectureInterfaceMonitoringView : UserControl
{
    public ArchitectureInterfaceMonitoringView()
    {
        this.DataContext = new ArchitectureInterfaceMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ArchitectureInterfaceMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}