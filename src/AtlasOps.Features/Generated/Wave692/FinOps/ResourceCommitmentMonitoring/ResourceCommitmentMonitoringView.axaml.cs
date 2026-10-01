namespace AtlasOps.Features.FinOps.ResourceCommitmentMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ResourceCommitmentMonitoringView : UserControl
{
    public ResourceCommitmentMonitoringView()
    {
        this.DataContext = new ResourceCommitmentMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ResourceCommitmentMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}