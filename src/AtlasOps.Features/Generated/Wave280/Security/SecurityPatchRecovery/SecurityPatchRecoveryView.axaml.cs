namespace AtlasOps.Features.Security.SecurityPatchRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityPatchRecoveryView : UserControl
{
    public SecurityPatchRecoveryView()
    {
        this.DataContext = new SecurityPatchRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityPatchRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}