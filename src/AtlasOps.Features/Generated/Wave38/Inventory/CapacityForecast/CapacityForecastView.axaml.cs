namespace AtlasOps.Features.Inventory.CapacityForecast;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CapacityForecastView : UserControl
{
    public CapacityForecastView()
    {
        this.DataContext = new CapacityForecastViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CapacityForecastViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}