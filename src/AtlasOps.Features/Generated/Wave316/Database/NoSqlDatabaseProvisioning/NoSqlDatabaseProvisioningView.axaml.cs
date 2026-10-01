namespace AtlasOps.Features.Database.NoSqlDatabaseProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NoSqlDatabaseProvisioningView : UserControl
{
    public NoSqlDatabaseProvisioningView()
    {
        this.DataContext = new NoSqlDatabaseProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NoSqlDatabaseProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}