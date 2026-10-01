namespace AtlasOps.Features.Api.ApiHealthMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiHealthMonitoringView : UserControl
{
    public ApiHealthMonitoringView()
    {
        this.DataContext = new ApiHealthMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiHealthMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}