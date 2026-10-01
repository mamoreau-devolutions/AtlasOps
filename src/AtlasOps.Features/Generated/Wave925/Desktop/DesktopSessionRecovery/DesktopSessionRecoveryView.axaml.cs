namespace AtlasOps.Features.Desktop.DesktopSessionRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopSessionRecoveryView : UserControl
{
    public DesktopSessionRecoveryView()
    {
        this.DataContext = new DesktopSessionRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopSessionRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}