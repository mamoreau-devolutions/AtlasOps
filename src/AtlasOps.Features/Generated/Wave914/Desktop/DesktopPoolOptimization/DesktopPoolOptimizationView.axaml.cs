namespace AtlasOps.Features.Desktop.DesktopPoolOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopPoolOptimizationView : UserControl
{
    public DesktopPoolOptimizationView()
    {
        this.DataContext = new DesktopPoolOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopPoolOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}