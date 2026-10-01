namespace AtlasOps.Features.Compute.ComputeReservationOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeReservationOptimizationView : UserControl
{
    public ComputeReservationOptimizationView()
    {
        this.DataContext = new ComputeReservationOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeReservationOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}