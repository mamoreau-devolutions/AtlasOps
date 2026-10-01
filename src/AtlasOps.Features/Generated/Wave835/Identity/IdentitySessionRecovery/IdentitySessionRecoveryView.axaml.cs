namespace AtlasOps.Features.Identity.IdentitySessionRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentitySessionRecoveryView : UserControl
{
    public IdentitySessionRecoveryView()
    {
        this.DataContext = new IdentitySessionRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentitySessionRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}