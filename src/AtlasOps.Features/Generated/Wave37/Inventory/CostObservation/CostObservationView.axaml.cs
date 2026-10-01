namespace AtlasOps.Features.Inventory.CostObservation;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CostObservationView : UserControl
{
    public CostObservationView()
    {
        this.DataContext = new CostObservationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CostObservationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}