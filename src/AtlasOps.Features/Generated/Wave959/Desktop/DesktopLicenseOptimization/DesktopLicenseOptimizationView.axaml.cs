namespace AtlasOps.Features.Desktop.DesktopLicenseOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopLicenseOptimizationView : UserControl
{
    public DesktopLicenseOptimizationView()
    {
        this.DataContext = new DesktopLicenseOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopLicenseOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}