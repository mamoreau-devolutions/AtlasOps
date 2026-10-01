namespace AtlasOps.Features.Edge.EdgeUpdateMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeUpdateMonitoringView : UserControl
{
    public EdgeUpdateMonitoringView()
    {
        this.DataContext = new EdgeUpdateMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeUpdateMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}