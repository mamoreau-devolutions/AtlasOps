namespace AtlasOps.Features.Database.SqlDatabaseGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SqlDatabaseGovernanceView : UserControl
{
    public SqlDatabaseGovernanceView()
    {
        this.DataContext = new SqlDatabaseGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SqlDatabaseGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}