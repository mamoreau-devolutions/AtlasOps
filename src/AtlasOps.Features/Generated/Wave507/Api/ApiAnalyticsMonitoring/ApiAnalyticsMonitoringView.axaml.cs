namespace AtlasOps.Features.Api.ApiAnalyticsMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiAnalyticsMonitoringView : UserControl
{
    public ApiAnalyticsMonitoringView()
    {
        this.DataContext = new ApiAnalyticsMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiAnalyticsMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}