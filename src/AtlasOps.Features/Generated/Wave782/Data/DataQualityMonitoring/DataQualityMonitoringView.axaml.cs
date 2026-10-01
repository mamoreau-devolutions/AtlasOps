namespace AtlasOps.Features.Data.DataQualityMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataQualityMonitoringView : UserControl
{
    public DataQualityMonitoringView()
    {
        this.DataContext = new DataQualityMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataQualityMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}