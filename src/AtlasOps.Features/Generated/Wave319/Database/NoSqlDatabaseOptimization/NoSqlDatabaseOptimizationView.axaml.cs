namespace AtlasOps.Features.Database.NoSqlDatabaseOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NoSqlDatabaseOptimizationView : UserControl
{
    public NoSqlDatabaseOptimizationView()
    {
        this.DataContext = new NoSqlDatabaseOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NoSqlDatabaseOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}