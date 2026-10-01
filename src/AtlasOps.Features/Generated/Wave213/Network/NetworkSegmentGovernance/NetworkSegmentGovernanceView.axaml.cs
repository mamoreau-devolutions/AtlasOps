namespace AtlasOps.Features.Network.NetworkSegmentGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkSegmentGovernanceView : UserControl
{
    public NetworkSegmentGovernanceView()
    {
        this.DataContext = new NetworkSegmentGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkSegmentGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}