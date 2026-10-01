namespace AtlasOps.Features.Api.ApiQuotaMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiQuotaMonitoringView : UserControl
{
    public ApiQuotaMonitoringView()
    {
        this.DataContext = new ApiQuotaMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiQuotaMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}