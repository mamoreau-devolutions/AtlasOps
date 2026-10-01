namespace AtlasOps.Features.Identity.IdentitySessionOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentitySessionOptimizationView : UserControl
{
    public IdentitySessionOptimizationView()
    {
        this.DataContext = new IdentitySessionOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentitySessionOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}