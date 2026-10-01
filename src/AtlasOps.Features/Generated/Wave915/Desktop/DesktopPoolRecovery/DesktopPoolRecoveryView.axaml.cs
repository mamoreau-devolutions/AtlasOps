namespace AtlasOps.Features.Desktop.DesktopPoolRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopPoolRecoveryView : UserControl
{
    public DesktopPoolRecoveryView()
    {
        this.DataContext = new DesktopPoolRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopPoolRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}