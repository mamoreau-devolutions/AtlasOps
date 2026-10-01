namespace AtlasOps.Features.Identity.IdentityClaimOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityClaimOptimizationView : UserControl
{
    public IdentityClaimOptimizationView()
    {
        this.DataContext = new IdentityClaimOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityClaimOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}