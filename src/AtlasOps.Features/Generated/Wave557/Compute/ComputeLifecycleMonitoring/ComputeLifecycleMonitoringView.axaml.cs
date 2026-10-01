namespace AtlasOps.Features.Compute.ComputeLifecycleMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeLifecycleMonitoringView : UserControl
{
    public ComputeLifecycleMonitoringView()
    {
        this.DataContext = new ComputeLifecycleMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeLifecycleMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}