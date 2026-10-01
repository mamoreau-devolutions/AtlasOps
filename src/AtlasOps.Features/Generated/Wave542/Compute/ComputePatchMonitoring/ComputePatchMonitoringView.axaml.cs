namespace AtlasOps.Features.Compute.ComputePatchMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputePatchMonitoringView : UserControl
{
    public ComputePatchMonitoringView()
    {
        this.DataContext = new ComputePatchMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputePatchMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}