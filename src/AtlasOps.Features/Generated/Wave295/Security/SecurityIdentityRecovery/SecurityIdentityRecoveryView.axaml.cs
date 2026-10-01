namespace AtlasOps.Features.Security.SecurityIdentityRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityIdentityRecoveryView : UserControl
{
    public SecurityIdentityRecoveryView()
    {
        this.DataContext = new SecurityIdentityRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityIdentityRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}