namespace AtlasOps.Features.Identity.IdentityApplicationRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityApplicationRecoveryView : UserControl
{
    public IdentityApplicationRecoveryView()
    {
        this.DataContext = new IdentityApplicationRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityApplicationRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}