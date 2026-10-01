namespace AtlasOps.Features.Edge.EdgeTelemetryGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeTelemetryGovernanceView : UserControl
{
    public EdgeTelemetryGovernanceView()
    {
        this.DataContext = new EdgeTelemetryGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeTelemetryGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}