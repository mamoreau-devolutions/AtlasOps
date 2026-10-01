namespace AtlasOps.Features.Kubernetes.KubernetesPodOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesPodOptimizationView : UserControl
{
    public KubernetesPodOptimizationView()
    {
        this.DataContext = new KubernetesPodOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesPodOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}