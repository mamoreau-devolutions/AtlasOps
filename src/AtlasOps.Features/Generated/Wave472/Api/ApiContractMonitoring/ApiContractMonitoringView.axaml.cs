namespace AtlasOps.Features.Api.ApiContractMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiContractMonitoringView : UserControl
{
    public ApiContractMonitoringView()
    {
        this.DataContext = new ApiContractMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiContractMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}