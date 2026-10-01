namespace AtlasOps.Features.Network.NetworkPolicyGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkPolicyGovernanceView : UserControl
{
    public NetworkPolicyGovernanceView()
    {
        this.DataContext = new NetworkPolicyGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkPolicyGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}