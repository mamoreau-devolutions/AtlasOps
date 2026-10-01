namespace AtlasOps.Features.Data.DataLineageMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataLineageMonitoringView : UserControl
{
    public DataLineageMonitoringView()
    {
        this.DataContext = new DataLineageMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataLineageMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}