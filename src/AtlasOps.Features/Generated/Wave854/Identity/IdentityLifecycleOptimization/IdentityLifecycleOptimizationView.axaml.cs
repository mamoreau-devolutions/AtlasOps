namespace AtlasOps.Features.Identity.IdentityLifecycleOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityLifecycleOptimizationView : UserControl
{
    public IdentityLifecycleOptimizationView()
    {
        this.DataContext = new IdentityLifecycleOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityLifecycleOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}