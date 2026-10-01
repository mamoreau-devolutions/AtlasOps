namespace AtlasOps.Features.Security.SecurityIdentityOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityIdentityOptimizationView : UserControl
{
    public SecurityIdentityOptimizationView()
    {
        this.DataContext = new SecurityIdentityOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityIdentityOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}