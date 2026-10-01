namespace AtlasOps.Features.Identity.IdentityProviderRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityProviderRecoveryView : UserControl
{
    public IdentityProviderRecoveryView()
    {
        this.DataContext = new IdentityProviderRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityProviderRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}