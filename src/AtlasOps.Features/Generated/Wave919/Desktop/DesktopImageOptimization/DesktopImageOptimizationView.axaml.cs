namespace AtlasOps.Features.Desktop.DesktopImageOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopImageOptimizationView : UserControl
{
    public DesktopImageOptimizationView()
    {
        this.DataContext = new DesktopImageOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopImageOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}