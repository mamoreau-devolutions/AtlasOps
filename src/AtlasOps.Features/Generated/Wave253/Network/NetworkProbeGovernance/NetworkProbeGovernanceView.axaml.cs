namespace AtlasOps.Features.Network.NetworkProbeGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkProbeGovernanceView : UserControl
{
    public NetworkProbeGovernanceView()
    {
        this.DataContext = new NetworkProbeGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkProbeGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}