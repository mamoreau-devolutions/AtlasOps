namespace AtlasOps.Features.Kubernetes.KubernetesWorkloadGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesWorkloadGovernanceView : UserControl
{
    public KubernetesWorkloadGovernanceView()
    {
        this.DataContext = new KubernetesWorkloadGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesWorkloadGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}