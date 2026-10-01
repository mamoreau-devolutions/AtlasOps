namespace AtlasOps.Features.Delivery.ReleaseCalendarRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseCalendarRecoveryView : UserControl
{
    public ReleaseCalendarRecoveryView()
    {
        this.DataContext = new ReleaseCalendarRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseCalendarRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}