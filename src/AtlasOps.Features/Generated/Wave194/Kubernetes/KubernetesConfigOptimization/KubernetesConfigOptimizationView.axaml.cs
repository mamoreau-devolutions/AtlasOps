namespace AtlasOps.Features.Kubernetes.KubernetesConfigOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesConfigOptimizationView : UserControl
{
    public KubernetesConfigOptimizationView()
    {
        this.DataContext = new KubernetesConfigOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesConfigOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}