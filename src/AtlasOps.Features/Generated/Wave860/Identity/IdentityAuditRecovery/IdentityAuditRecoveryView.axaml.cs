namespace AtlasOps.Features.Identity.IdentityAuditRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityAuditRecoveryView : UserControl
{
    public IdentityAuditRecoveryView()
    {
        this.DataContext = new IdentityAuditRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityAuditRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}