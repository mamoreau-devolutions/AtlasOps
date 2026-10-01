namespace AtlasOps.Features.Identity.IdentityUserRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityUserRecoveryView : UserControl
{
    public IdentityUserRecoveryView()
    {
        this.DataContext = new IdentityUserRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityUserRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}