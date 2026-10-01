namespace AtlasOps.Features.Security.SecurityScanRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityScanRecoveryView : UserControl
{
    public SecurityScanRecoveryView()
    {
        this.DataContext = new SecurityScanRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityScanRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}