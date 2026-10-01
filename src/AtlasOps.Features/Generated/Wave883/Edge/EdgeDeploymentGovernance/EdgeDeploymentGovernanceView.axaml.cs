namespace AtlasOps.Features.Edge.EdgeDeploymentGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeDeploymentGovernanceView : UserControl
{
    public EdgeDeploymentGovernanceView()
    {
        this.DataContext = new EdgeDeploymentGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeDeploymentGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}