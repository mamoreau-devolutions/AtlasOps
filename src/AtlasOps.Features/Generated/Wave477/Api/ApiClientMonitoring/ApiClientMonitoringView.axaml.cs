namespace AtlasOps.Features.Api.ApiClientMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiClientMonitoringView : UserControl
{
    public ApiClientMonitoringView()
    {
        this.DataContext = new ApiClientMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiClientMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}