namespace AtlasOps.Features.Desktop.DesktopUpdateRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopUpdateRecoveryView : UserControl
{
    public DesktopUpdateRecoveryView()
    {
        this.DataContext = new DesktopUpdateRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopUpdateRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}