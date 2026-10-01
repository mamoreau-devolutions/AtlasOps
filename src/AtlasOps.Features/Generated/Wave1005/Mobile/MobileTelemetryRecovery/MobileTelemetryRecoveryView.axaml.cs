namespace AtlasOps.Features.Mobile.MobileTelemetryRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileTelemetryRecoveryView : UserControl
{
    public MobileTelemetryRecoveryView()
    {
        this.DataContext = new MobileTelemetryRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileTelemetryRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}