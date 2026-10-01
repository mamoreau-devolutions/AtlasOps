namespace AtlasOps.Features.Network.NetworkSegmentProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NetworkSegmentProvisioningView : UserControl
{
    public NetworkSegmentProvisioningView()
    {
        this.DataContext = new NetworkSegmentProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NetworkSegmentProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}