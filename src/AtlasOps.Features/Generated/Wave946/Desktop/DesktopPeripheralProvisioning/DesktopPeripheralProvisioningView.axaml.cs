namespace AtlasOps.Features.Desktop.DesktopPeripheralProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopPeripheralProvisioningView : UserControl
{
    public DesktopPeripheralProvisioningView()
    {
        this.DataContext = new DesktopPeripheralProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopPeripheralProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}