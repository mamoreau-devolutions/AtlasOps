namespace AtlasOps.Features.Compute.ComputeImageMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeImageMonitoringView : UserControl
{
    public ComputeImageMonitoringView()
    {
        this.DataContext = new ComputeImageMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeImageMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}