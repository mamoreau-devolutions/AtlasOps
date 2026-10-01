namespace AtlasOps.Features.Database.DatabaseRestoreRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseRestoreRecoveryView : UserControl
{
    public DatabaseRestoreRecoveryView()
    {
        this.DataContext = new DatabaseRestoreRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseRestoreRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}