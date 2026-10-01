namespace AtlasOps.Features.Identity.IdentityClaimRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityClaimRecoveryView : UserControl
{
    public IdentityClaimRecoveryView()
    {
        this.DataContext = new IdentityClaimRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityClaimRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}