namespace AtlasOps.Features.Desktop.DesktopPeripheralOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopPeripheralOptimizationView : UserControl
{
    public DesktopPeripheralOptimizationView()
    {
        this.DataContext = new DesktopPeripheralOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopPeripheralOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}