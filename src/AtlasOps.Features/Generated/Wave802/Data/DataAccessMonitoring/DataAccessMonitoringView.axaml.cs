namespace AtlasOps.Features.Data.DataAccessMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataAccessMonitoringView : UserControl
{
    public DataAccessMonitoringView()
    {
        this.DataContext = new DataAccessMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataAccessMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}