namespace AtlasOps.Features.Database.SqlDatabaseRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SqlDatabaseRecoveryView : UserControl
{
    public SqlDatabaseRecoveryView()
    {
        this.DataContext = new SqlDatabaseRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SqlDatabaseRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}