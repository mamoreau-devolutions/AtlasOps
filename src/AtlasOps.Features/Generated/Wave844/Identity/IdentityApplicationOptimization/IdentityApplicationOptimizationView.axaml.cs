namespace AtlasOps.Features.Identity.IdentityApplicationOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityApplicationOptimizationView : UserControl
{
    public IdentityApplicationOptimizationView()
    {
        this.DataContext = new IdentityApplicationOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityApplicationOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}