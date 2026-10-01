namespace AtlasOps.Features.Kubernetes.KubernetesServiceOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesServiceOptimizationView : UserControl
{
    public KubernetesServiceOptimizationView()
    {
        this.DataContext = new KubernetesServiceOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesServiceOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}