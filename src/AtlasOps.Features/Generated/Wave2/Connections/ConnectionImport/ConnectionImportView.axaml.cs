namespace AtlasOps.Features.Connections.ConnectionImport;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ConnectionImportView : UserControl
{
    public ConnectionImportView()
    {
        this.DataContext = new ConnectionImportViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ConnectionImportViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}