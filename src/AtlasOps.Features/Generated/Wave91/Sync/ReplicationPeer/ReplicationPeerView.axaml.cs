namespace AtlasOps.Features.Sync.ReplicationPeer;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReplicationPeerView : UserControl
{
    public ReplicationPeerView()
    {
        this.DataContext = new ReplicationPeerViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReplicationPeerViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}