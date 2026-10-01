namespace AtlasOps.Features.Mobile.MobilePolicyProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobilePolicyProvisioningView : UserControl
{
    public MobilePolicyProvisioningView()
    {
        this.DataContext = new MobilePolicyProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobilePolicyProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}