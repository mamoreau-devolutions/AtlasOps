namespace AtlasOps.Features.Mobile.MobileDeviceProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileDeviceProvisioningView : UserControl
{
    public MobileDeviceProvisioningView()
    {
        this.DataContext = new MobileDeviceProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileDeviceProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}