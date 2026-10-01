namespace AtlasOps.Features.Data.DataTransformMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataTransformMonitoringView : UserControl
{
    public DataTransformMonitoringView()
    {
        this.DataContext = new DataTransformMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataTransformMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}