namespace AtlasOps.Features.Data.DataRetentionMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataRetentionMonitoringView : UserControl
{
    public DataRetentionMonitoringView()
    {
        this.DataContext = new DataRetentionMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataRetentionMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}