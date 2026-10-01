namespace AtlasOps.Features.Edge.EdgeTelemetryRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeTelemetryRecoveryView : UserControl
{
    public EdgeTelemetryRecoveryView()
    {
        this.DataContext = new EdgeTelemetryRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeTelemetryRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}