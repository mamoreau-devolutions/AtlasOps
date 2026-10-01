namespace AtlasOps.Features.Security.SecurityPatchOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityPatchOptimizationView : UserControl
{
    public SecurityPatchOptimizationView()
    {
        this.DataContext = new SecurityPatchOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityPatchOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}