namespace AtlasOps.Features.Identity.IdentityFactorOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityFactorOptimizationView : UserControl
{
    public IdentityFactorOptimizationView()
    {
        this.DataContext = new IdentityFactorOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityFactorOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}