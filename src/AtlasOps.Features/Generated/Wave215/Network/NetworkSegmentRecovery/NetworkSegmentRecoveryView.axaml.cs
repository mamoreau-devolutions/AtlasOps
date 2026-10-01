namespace AtlasOps.Features.Network.NetworkSegmentRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkSegmentRecoveryView : UserControl
{
    public NetworkSegmentRecoveryView()
    {
        this.DataContext = new NetworkSegmentRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkSegmentRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}