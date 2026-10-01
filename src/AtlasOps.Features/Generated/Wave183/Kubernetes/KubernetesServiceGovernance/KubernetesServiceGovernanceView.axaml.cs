namespace AtlasOps.Features.Kubernetes.KubernetesServiceGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesServiceGovernanceView : UserControl
{
    public KubernetesServiceGovernanceView()
    {
        this.DataContext = new KubernetesServiceGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesServiceGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}