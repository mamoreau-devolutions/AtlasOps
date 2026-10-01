namespace AtlasOps.Features.Desktop.DesktopHealthOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopHealthOptimizationView : UserControl
{
    public DesktopHealthOptimizationView()
    {
        this.DataContext = new DesktopHealthOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopHealthOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}