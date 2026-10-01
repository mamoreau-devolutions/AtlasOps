namespace AtlasOps.Features.Identity.IdentityLifecycleRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityLifecycleRecoveryView : UserControl
{
    public IdentityLifecycleRecoveryView()
    {
        this.DataContext = new IdentityLifecycleRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityLifecycleRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}