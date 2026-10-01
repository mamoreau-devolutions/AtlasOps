namespace AtlasOps.Features.Mobile.MobileFleetOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileFleetOptimizationView : UserControl
{
    public MobileFleetOptimizationView()
    {
        this.DataContext = new MobileFleetOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileFleetOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}