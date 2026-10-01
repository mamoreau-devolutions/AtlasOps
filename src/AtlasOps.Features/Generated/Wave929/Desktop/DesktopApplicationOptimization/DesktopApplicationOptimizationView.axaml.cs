namespace AtlasOps.Features.Desktop.DesktopApplicationOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopApplicationOptimizationView : UserControl
{
    public DesktopApplicationOptimizationView()
    {
        this.DataContext = new DesktopApplicationOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopApplicationOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}