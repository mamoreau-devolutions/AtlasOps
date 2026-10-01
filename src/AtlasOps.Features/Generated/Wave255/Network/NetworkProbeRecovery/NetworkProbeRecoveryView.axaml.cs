namespace AtlasOps.Features.Network.NetworkProbeRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkProbeRecoveryView : UserControl
{
    public NetworkProbeRecoveryView()
    {
        this.DataContext = new NetworkProbeRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkProbeRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}