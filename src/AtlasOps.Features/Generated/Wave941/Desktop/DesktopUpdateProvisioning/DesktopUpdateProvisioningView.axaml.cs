namespace AtlasOps.Features.Desktop.DesktopUpdateProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopUpdateProvisioningView : UserControl
{
    public DesktopUpdateProvisioningView()
    {
        this.DataContext = new DesktopUpdateProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopUpdateProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}