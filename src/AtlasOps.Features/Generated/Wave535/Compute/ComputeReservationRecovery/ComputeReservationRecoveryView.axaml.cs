namespace AtlasOps.Features.Compute.ComputeReservationRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeReservationRecoveryView : UserControl
{
    public ComputeReservationRecoveryView()
    {
        this.DataContext = new ComputeReservationRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeReservationRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}