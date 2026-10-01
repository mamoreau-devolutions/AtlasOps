namespace AtlasOps.Features.Desktop.DesktopHealthRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopHealthRecoveryView : UserControl
{
    public DesktopHealthRecoveryView()
    {
        this.DataContext = new DesktopHealthRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopHealthRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}