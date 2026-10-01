namespace AtlasOps.Features.Data.DataProductMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DataProductMonitoringView : UserControl
{
    public DataProductMonitoringView()
    {
        this.DataContext = new DataProductMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DataProductMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}