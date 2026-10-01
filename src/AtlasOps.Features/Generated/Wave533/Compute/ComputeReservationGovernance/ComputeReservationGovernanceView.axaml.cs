namespace AtlasOps.Features.Compute.ComputeReservationGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeReservationGovernanceView : UserControl
{
    public ComputeReservationGovernanceView()
    {
        this.DataContext = new ComputeReservationGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeReservationGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}