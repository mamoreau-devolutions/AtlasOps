namespace AtlasOps.Features.Edge.EdgeIncidentRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeIncidentRecoveryView : UserControl
{
    public EdgeIncidentRecoveryView()
    {
        this.DataContext = new EdgeIncidentRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeIncidentRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}