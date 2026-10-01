namespace AtlasOps.Features.Database.SqlDatabaseProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SqlDatabaseProvisioningView : UserControl
{
    public SqlDatabaseProvisioningView()
    {
        this.DataContext = new SqlDatabaseProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SqlDatabaseProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}