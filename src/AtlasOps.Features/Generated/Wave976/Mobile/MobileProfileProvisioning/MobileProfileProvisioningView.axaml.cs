namespace AtlasOps.Features.Mobile.MobileProfileProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileProfileProvisioningView : UserControl
{
    public MobileProfileProvisioningView()
    {
        this.DataContext = new MobileProfileProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileProfileProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}