namespace AtlasOps.Features.Identity.IdentityGroupOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityGroupOptimizationView : UserControl
{
    public IdentityGroupOptimizationView()
    {
        this.DataContext = new IdentityGroupOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityGroupOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}