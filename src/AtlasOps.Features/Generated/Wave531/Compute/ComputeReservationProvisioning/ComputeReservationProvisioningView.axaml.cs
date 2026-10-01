namespace AtlasOps.Features.Compute.ComputeReservationProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeReservationProvisioningView : UserControl
{
    public ComputeReservationProvisioningView()
    {
        this.DataContext = new ComputeReservationProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeReservationProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}