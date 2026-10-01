namespace AtlasOps.Features.Connections.FileTransfer;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class FileTransferView : UserControl
{
    public FileTransferView()
    {
        this.DataContext = new FileTransferViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is FileTransferViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}