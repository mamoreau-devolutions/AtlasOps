namespace AtlasOps.Features.Kubernetes.KubernetesPodMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KubernetesPodMonitoringView : UserControl
{
    public KubernetesPodMonitoringView()
    {
        this.DataContext = new KubernetesPodMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KubernetesPodMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}