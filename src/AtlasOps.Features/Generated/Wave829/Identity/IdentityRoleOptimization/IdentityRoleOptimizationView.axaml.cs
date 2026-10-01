namespace AtlasOps.Features.Identity.IdentityRoleOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityRoleOptimizationView : UserControl
{
    public IdentityRoleOptimizationView()
    {
        this.DataContext = new IdentityRoleOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityRoleOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}