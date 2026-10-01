namespace AtlasOps.Features.Desktop.DesktopPeripheralRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopPeripheralRecoveryView : UserControl
{
    public DesktopPeripheralRecoveryView()
    {
        this.DataContext = new DesktopPeripheralRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopPeripheralRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}