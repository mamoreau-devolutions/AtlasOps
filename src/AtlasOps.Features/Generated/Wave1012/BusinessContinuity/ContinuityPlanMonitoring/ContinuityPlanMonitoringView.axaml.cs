namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ContinuityPlanMonitoringView : UserControl
{
    public ContinuityPlanMonitoringView()
    {
        this.DataContext = new ContinuityPlanMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ContinuityPlanMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}