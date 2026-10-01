namespace AtlasOps.Features.Network.NetworkAddressGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkAddressGovernanceView : UserControl
{
    public NetworkAddressGovernanceView()
    {
        this.DataContext = new NetworkAddressGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkAddressGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}