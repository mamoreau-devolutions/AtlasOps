namespace AtlasOps.Features.Edge.EdgePolicyMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgePolicyMonitoringView : UserControl
{
    public EdgePolicyMonitoringView()
    {
        this.DataContext = new EdgePolicyMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgePolicyMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}