namespace AtlasOps.Features.Kubernetes.KubernetesNamespaceGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesNamespaceGovernanceView : UserControl
{
    public KubernetesNamespaceGovernanceView()
    {
        this.DataContext = new KubernetesNamespaceGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesNamespaceGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}