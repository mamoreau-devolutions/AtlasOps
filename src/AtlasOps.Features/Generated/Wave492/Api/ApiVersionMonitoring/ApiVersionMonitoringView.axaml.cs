namespace AtlasOps.Features.Api.ApiVersionMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiVersionMonitoringView : UserControl
{
    public ApiVersionMonitoringView()
    {
        this.DataContext = new ApiVersionMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiVersionMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}