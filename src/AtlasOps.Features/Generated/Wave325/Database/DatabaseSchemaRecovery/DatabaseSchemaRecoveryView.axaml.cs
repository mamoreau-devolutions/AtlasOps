namespace AtlasOps.Features.Database.DatabaseSchemaRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseSchemaRecoveryView : UserControl
{
    public DatabaseSchemaRecoveryView()
    {
        this.DataContext = new DatabaseSchemaRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseSchemaRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}