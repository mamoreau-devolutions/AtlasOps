namespace AtlasOps.Features.Desktop.DesktopProfileOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopProfileOptimizationView : UserControl
{
    public DesktopProfileOptimizationView()
    {
        this.DataContext = new DesktopProfileOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopProfileOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}