namespace AtlasOps.Features.Mobile.MobileUpdateProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileUpdateProvisioningView : UserControl
{
    public MobileUpdateProvisioningView()
    {
        this.DataContext = new MobileUpdateProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileUpdateProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}