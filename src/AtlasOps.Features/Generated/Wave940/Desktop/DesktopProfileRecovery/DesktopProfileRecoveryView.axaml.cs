namespace AtlasOps.Features.Desktop.DesktopProfileRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopProfileRecoveryView : UserControl
{
    public DesktopProfileRecoveryView()
    {
        this.DataContext = new DesktopProfileRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopProfileRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}