namespace AtlasOps.Features.Identity.IdentityGroupRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityGroupRecoveryView : UserControl
{
    public IdentityGroupRecoveryView()
    {
        this.DataContext = new IdentityGroupRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityGroupRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}