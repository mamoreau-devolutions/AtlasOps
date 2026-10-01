namespace AtlasOps.Features.Database.NoSqlDatabaseGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NoSqlDatabaseGovernanceView : UserControl
{
    public NoSqlDatabaseGovernanceView()
    {
        this.DataContext = new NoSqlDatabaseGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NoSqlDatabaseGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}