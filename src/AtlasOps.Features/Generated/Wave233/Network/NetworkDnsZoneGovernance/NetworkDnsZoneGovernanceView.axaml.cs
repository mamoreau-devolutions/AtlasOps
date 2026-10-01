namespace AtlasOps.Features.Network.NetworkDnsZoneGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkDnsZoneGovernanceView : UserControl
{
    public NetworkDnsZoneGovernanceView()
    {
        this.DataContext = new NetworkDnsZoneGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkDnsZoneGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}