namespace AtlasOps.Features.Network.NetworkPolicyMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkPolicyMonitoringView : UserControl
{
    public NetworkPolicyMonitoringView()
    {
        this.DataContext = new NetworkPolicyMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkPolicyMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}