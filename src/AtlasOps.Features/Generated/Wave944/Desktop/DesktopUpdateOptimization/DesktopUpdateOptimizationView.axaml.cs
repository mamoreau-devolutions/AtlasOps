namespace AtlasOps.Features.Desktop.DesktopUpdateOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopUpdateOptimizationView : UserControl
{
    public DesktopUpdateOptimizationView()
    {
        this.DataContext = new DesktopUpdateOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopUpdateOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}