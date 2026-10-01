namespace AtlasOps.Features.Mobile.MobileFleetProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileFleetProvisioningView : UserControl
{
    public MobileFleetProvisioningView()
    {
        this.DataContext = new MobileFleetProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileFleetProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}