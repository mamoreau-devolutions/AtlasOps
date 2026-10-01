namespace AtlasOps.Features.Edge.EdgeNetworkGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeNetworkGovernanceView : UserControl
{
    public EdgeNetworkGovernanceView()
    {
        this.DataContext = new EdgeNetworkGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeNetworkGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}