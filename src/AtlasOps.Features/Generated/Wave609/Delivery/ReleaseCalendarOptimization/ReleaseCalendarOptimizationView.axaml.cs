namespace AtlasOps.Features.Delivery.ReleaseCalendarOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseCalendarOptimizationView : UserControl
{
    public ReleaseCalendarOptimizationView()
    {
        this.DataContext = new ReleaseCalendarOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseCalendarOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}