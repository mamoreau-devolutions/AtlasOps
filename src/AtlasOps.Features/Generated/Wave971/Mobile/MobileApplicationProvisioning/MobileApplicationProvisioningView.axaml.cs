namespace AtlasOps.Features.Mobile.MobileApplicationProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileApplicationProvisioningView : UserControl
{
    public MobileApplicationProvisioningView()
    {
        this.DataContext = new MobileApplicationProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileApplicationProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}