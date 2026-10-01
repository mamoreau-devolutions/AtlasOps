namespace AtlasOps.Features.Inventory.HealthObservation;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class HealthObservationView : UserControl
{
    public HealthObservationView()
    {
        this.DataContext = new HealthObservationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is HealthObservationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}