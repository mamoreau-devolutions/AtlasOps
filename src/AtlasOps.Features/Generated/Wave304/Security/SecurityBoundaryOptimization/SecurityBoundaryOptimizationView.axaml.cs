namespace AtlasOps.Features.Security.SecurityBoundaryOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityBoundaryOptimizationView : UserControl
{
    public SecurityBoundaryOptimizationView()
    {
        this.DataContext = new SecurityBoundaryOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityBoundaryOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}