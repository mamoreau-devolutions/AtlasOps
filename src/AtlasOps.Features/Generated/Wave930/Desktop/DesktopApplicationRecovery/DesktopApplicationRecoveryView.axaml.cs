namespace AtlasOps.Features.Desktop.DesktopApplicationRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DesktopApplicationRecoveryView : UserControl
{
    public DesktopApplicationRecoveryView()
    {
        this.DataContext = new DesktopApplicationRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DesktopApplicationRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}