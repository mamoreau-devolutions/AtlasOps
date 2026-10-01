namespace AtlasOps.Features.Security.SecurityBaselineOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityBaselineOptimizationView : UserControl
{
    public SecurityBaselineOptimizationView()
    {
        this.DataContext = new SecurityBaselineOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityBaselineOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}