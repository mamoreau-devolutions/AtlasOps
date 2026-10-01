namespace AtlasOps.Features.Network.NetworkVpnGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkVpnGovernanceView : UserControl
{
    public NetworkVpnGovernanceView()
    {
        this.DataContext = new NetworkVpnGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkVpnGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}