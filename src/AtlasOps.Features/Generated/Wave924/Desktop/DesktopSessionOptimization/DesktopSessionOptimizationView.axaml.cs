namespace AtlasOps.Features.Desktop.DesktopSessionOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopSessionOptimizationView : UserControl
{
    public DesktopSessionOptimizationView()
    {
        this.DataContext = new DesktopSessionOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopSessionOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}