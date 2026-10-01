namespace AtlasOps.Features.Compute.ComputeReservationMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeReservationMonitoringView : UserControl
{
    public ComputeReservationMonitoringView()
    {
        this.DataContext = new ComputeReservationMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeReservationMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}