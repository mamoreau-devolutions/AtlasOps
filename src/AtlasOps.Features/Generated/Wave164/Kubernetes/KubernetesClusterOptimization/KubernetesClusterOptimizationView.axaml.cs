namespace AtlasOps.Features.Kubernetes.KubernetesClusterOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesClusterOptimizationView : UserControl
{
    public KubernetesClusterOptimizationView()
    {
        this.DataContext = new KubernetesClusterOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesClusterOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}