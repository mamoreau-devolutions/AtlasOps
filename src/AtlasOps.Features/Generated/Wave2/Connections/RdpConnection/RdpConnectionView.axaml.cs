namespace AtlasOps.Features.Connections.RdpConnection;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RdpConnectionView : UserControl
{
    public RdpConnectionView()
    {
        this.DataContext = new RdpConnectionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RdpConnectionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}