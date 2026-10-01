namespace AtlasOps.Features.Identity.IdentityFactorRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityFactorRecoveryView : UserControl
{
    public IdentityFactorRecoveryView()
    {
        this.DataContext = new IdentityFactorRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityFactorRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}