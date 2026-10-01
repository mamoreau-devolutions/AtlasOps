namespace AtlasOps.Features.Delivery.ReleaseApprovalMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseApprovalMonitoringView : UserControl
{
    public ReleaseApprovalMonitoringView()
    {
        this.DataContext = new ReleaseApprovalMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseApprovalMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}