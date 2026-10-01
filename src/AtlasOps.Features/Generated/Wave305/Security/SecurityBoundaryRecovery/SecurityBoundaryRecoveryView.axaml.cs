namespace AtlasOps.Features.Security.SecurityBoundaryRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityBoundaryRecoveryView : UserControl
{
    public SecurityBoundaryRecoveryView()
    {
        this.DataContext = new SecurityBoundaryRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityBoundaryRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}