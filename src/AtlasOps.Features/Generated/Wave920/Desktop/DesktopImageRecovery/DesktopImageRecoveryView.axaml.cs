namespace AtlasOps.Features.Desktop.DesktopImageRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopImageRecoveryView : UserControl
{
    public DesktopImageRecoveryView()
    {
        this.DataContext = new DesktopImageRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopImageRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}