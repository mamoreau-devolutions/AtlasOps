namespace AtlasOps.Features.Security.SecurityFindingOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityFindingOptimizationView : UserControl
{
    public SecurityFindingOptimizationView()
    {
        this.DataContext = new SecurityFindingOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityFindingOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}