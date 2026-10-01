namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesWorkloadOptimizationView : UserControl
{
    public KubernetesWorkloadOptimizationView()
    {
        this.DataContext = new KubernetesWorkloadOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesWorkloadOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}