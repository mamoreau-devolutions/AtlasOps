namespace AtlasOps.Features.Connections.DatabaseConnection;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseConnectionView : UserControl
{
    public DatabaseConnectionView()
    {
        this.DataContext = new DatabaseConnectionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseConnectionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}