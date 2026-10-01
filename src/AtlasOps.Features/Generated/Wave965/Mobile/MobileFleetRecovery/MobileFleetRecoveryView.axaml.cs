namespace AtlasOps.Features.Mobile.MobileFleetRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileFleetRecoveryView : UserControl
{
    public MobileFleetRecoveryView()
    {
        this.DataContext = new MobileFleetRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileFleetRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}