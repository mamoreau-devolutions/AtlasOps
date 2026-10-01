namespace AtlasOps.Features.Identity.IdentityRoleRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityRoleRecoveryView : UserControl
{
    public IdentityRoleRecoveryView()
    {
        this.DataContext = new IdentityRoleRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityRoleRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}