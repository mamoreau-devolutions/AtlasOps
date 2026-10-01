namespace AtlasOps.Features.Security.SecuritySessionOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecuritySessionOptimizationView : UserControl
{
    public SecuritySessionOptimizationView()
    {
        this.DataContext = new SecuritySessionOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecuritySessionOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}