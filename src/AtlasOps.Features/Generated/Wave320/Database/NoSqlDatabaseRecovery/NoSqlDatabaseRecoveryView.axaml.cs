namespace AtlasOps.Features.Database.NoSqlDatabaseRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NoSqlDatabaseRecoveryView : UserControl
{
    public NoSqlDatabaseRecoveryView()
    {
        this.DataContext = new NoSqlDatabaseRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NoSqlDatabaseRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}