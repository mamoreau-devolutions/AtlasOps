namespace AtlasOps.Features.Kubernetes.KubernetesSecretOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesSecretOptimizationView : UserControl
{
    public KubernetesSecretOptimizationView()
    {
        this.DataContext = new KubernetesSecretOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesSecretOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}