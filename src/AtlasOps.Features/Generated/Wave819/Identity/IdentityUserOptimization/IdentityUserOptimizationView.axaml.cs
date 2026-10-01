namespace AtlasOps.Features.Identity.IdentityUserOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityUserOptimizationView : UserControl
{
    public IdentityUserOptimizationView()
    {
        this.DataContext = new IdentityUserOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityUserOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}