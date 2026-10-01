namespace AtlasOps.Features.Edge.EdgeGatewayGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeGatewayGovernanceView : UserControl
{
    public EdgeGatewayGovernanceView()
    {
        this.DataContext = new EdgeGatewayGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeGatewayGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}