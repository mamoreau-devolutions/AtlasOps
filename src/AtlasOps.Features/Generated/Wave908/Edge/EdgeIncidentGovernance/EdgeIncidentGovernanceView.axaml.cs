namespace AtlasOps.Features.Edge.EdgeIncidentGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeIncidentGovernanceView : UserControl
{
    public EdgeIncidentGovernanceView()
    {
        this.DataContext = new EdgeIncidentGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeIncidentGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}