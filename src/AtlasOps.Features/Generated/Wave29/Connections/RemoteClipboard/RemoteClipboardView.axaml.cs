namespace AtlasOps.Features.Connections.RemoteClipboard;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RemoteClipboardView : UserControl
{
    public RemoteClipboardView()
    {
        this.DataContext = new RemoteClipboardViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RemoteClipboardViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}