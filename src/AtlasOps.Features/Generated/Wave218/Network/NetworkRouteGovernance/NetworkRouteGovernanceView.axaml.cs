namespace AtlasOps.Features.Network.NetworkRouteGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkRouteGovernanceView : UserControl
{
    public NetworkRouteGovernanceView()
    {
        this.DataContext = new NetworkRouteGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkRouteGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}