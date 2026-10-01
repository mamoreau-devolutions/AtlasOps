namespace AtlasOps.Features.Security.SecurityBoundaryMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityBoundaryMonitoringView : UserControl
{
    public SecurityBoundaryMonitoringView()
    {
        this.DataContext = new SecurityBoundaryMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityBoundaryMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}