namespace AtlasOps.Features.Connections.PortForwarding;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class PortForwardingView : UserControl
{
    public PortForwardingView()
    {
        this.DataContext = new PortForwardingViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is PortForwardingViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}