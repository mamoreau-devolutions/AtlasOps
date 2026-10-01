namespace AtlasOps.Features.Edge.EdgeTelemetryProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeTelemetryProvisioningView : UserControl
{
    public EdgeTelemetryProvisioningView()
    {
        this.DataContext = new EdgeTelemetryProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeTelemetryProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}