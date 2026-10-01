namespace AtlasOps.Features.Edge.EdgeApplicationMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeApplicationMonitoringView : UserControl
{
    public EdgeApplicationMonitoringView()
    {
        this.DataContext = new EdgeApplicationMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeApplicationMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}