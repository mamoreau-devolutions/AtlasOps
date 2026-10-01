namespace AtlasOps.Features.Compute.ComputeConsoleMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeConsoleMonitoringView : UserControl
{
    public ComputeConsoleMonitoringView()
    {
        this.DataContext = new ComputeConsoleMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeConsoleMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}