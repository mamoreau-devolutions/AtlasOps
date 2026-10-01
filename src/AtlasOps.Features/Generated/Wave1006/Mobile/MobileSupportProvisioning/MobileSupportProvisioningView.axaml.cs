namespace AtlasOps.Features.Mobile.MobileSupportProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileSupportProvisioningView : UserControl
{
    public MobileSupportProvisioningView()
    {
        this.DataContext = new MobileSupportProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileSupportProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}