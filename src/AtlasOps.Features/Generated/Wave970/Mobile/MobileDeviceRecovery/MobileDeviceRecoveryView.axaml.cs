namespace AtlasOps.Features.Mobile.MobileDeviceRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileDeviceRecoveryView : UserControl
{
    public MobileDeviceRecoveryView()
    {
        this.DataContext = new MobileDeviceRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileDeviceRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}