namespace AtlasOps.Features.Connections.ConnectionPool;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ConnectionPoolView : UserControl
{
    public ConnectionPoolView()
    {
        this.DataContext = new ConnectionPoolViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ConnectionPoolViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}