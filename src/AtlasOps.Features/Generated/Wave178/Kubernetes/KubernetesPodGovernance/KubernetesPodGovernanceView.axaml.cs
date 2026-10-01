namespace AtlasOps.Features.Kubernetes.KubernetesPodGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesPodGovernanceView : UserControl
{
    public KubernetesPodGovernanceView()
    {
        this.DataContext = new KubernetesPodGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesPodGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}