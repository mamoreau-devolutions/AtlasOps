namespace AtlasOps.Features.Security.SecurityScanOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityScanOptimizationView : UserControl
{
    public SecurityScanOptimizationView()
    {
        this.DataContext = new SecurityScanOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityScanOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}