namespace AtlasOps.Features.Kubernetes.KubernetesIngressOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesIngressOptimizationView : UserControl
{
    public KubernetesIngressOptimizationView()
    {
        this.DataContext = new KubernetesIngressOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesIngressOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}