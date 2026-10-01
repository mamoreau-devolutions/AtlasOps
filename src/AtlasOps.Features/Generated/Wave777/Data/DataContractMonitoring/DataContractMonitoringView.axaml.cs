namespace AtlasOps.Features.Data.DataContractMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataContractMonitoringView : UserControl
{
    public DataContractMonitoringView()
    {
        this.DataContext = new DataContractMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataContractMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}