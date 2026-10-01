namespace AtlasOps.Features.Security.SecurityExceptionOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityExceptionOptimizationView : UserControl
{
    public SecurityExceptionOptimizationView()
    {
        this.DataContext = new SecurityExceptionOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityExceptionOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}