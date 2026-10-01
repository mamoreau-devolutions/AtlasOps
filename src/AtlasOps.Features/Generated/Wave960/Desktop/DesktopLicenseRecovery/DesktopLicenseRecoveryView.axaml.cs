namespace AtlasOps.Features.Desktop.DesktopLicenseRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopLicenseRecoveryView : UserControl
{
    public DesktopLicenseRecoveryView()
    {
        this.DataContext = new DesktopLicenseRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopLicenseRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}