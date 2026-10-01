namespace AtlasOps.Features.Api.ApiTokenMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiTokenMonitoringView : UserControl
{
    public ApiTokenMonitoringView()
    {
        this.DataContext = new ApiTokenMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiTokenMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}