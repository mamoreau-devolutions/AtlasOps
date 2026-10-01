namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesNamespaceOptimizationView : UserControl
{
    public KubernetesNamespaceOptimizationView()
    {
        this.DataContext = new KubernetesNamespaceOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesNamespaceOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}