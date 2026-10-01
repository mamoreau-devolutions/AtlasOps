namespace AtlasOps.Features.Data.DataDatasetMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataDatasetMonitoringView : UserControl
{
    public DataDatasetMonitoringView()
    {
        this.DataContext = new DataDatasetMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataDatasetMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}