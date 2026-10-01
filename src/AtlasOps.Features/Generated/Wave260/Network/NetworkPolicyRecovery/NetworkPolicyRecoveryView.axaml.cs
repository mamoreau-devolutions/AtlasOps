namespace AtlasOps.Features.Network.NetworkPolicyRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkPolicyRecoveryView : UserControl
{
    public NetworkPolicyRecoveryView()
    {
        this.DataContext = new NetworkPolicyRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkPolicyRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}