namespace AtlasOps.Features.Edge.EdgePolicyOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgePolicyOptimizationView : UserControl
{
    public EdgePolicyOptimizationView()
    {
        this.DataContext = new EdgePolicyOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgePolicyOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}