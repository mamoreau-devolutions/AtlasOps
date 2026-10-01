namespace AtlasOps.Features.Kubernetes.KubernetesOperatorOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesOperatorOptimizationView : UserControl
{
    public KubernetesOperatorOptimizationView()
    {
        this.DataContext = new KubernetesOperatorOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesOperatorOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}