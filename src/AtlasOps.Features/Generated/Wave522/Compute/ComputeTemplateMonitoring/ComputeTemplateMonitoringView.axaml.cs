namespace AtlasOps.Features.Compute.ComputeTemplateMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeTemplateMonitoringView : UserControl
{
    public ComputeTemplateMonitoringView()
    {
        this.DataContext = new ComputeTemplateMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeTemplateMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}