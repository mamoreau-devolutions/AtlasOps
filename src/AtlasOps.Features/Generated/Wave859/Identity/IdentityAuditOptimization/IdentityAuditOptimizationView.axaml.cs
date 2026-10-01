namespace AtlasOps.Features.Identity.IdentityAuditOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityAuditOptimizationView : UserControl
{
    public IdentityAuditOptimizationView()
    {
        this.DataContext = new IdentityAuditOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityAuditOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}