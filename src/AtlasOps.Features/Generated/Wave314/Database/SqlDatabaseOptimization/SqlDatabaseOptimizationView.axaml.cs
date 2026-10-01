namespace AtlasOps.Features.Database.SqlDatabaseOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SqlDatabaseOptimizationView : UserControl
{
    public SqlDatabaseOptimizationView()
    {
        this.DataContext = new SqlDatabaseOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SqlDatabaseOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}