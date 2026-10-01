namespace AtlasOps.Features.Edge.EdgeSiteGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeSiteGovernanceView : UserControl
{
    public EdgeSiteGovernanceView()
    {
        this.DataContext = new EdgeSiteGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeSiteGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}