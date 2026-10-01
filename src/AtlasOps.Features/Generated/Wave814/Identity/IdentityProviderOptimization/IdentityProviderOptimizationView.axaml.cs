namespace AtlasOps.Features.Identity.IdentityProviderOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityProviderOptimizationView : UserControl
{
    public IdentityProviderOptimizationView()
    {
        this.DataContext = new IdentityProviderOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityProviderOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}