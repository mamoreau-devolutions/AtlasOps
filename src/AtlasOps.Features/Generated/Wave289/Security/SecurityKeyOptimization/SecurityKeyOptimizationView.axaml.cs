namespace AtlasOps.Features.Security.SecurityKeyOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityKeyOptimizationView : UserControl
{
    public SecurityKeyOptimizationView()
    {
        this.DataContext = new SecurityKeyOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityKeyOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}