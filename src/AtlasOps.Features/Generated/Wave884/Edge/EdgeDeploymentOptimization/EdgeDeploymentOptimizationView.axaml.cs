namespace AtlasOps.Features.Edge.EdgeDeploymentOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeDeploymentOptimizationView : UserControl
{
    public EdgeDeploymentOptimizationView()
    {
        this.DataContext = new EdgeDeploymentOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeDeploymentOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}