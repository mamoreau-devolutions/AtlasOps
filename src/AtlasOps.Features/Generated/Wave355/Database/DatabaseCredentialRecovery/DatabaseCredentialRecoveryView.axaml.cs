namespace AtlasOps.Features.Database.DatabaseCredentialRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseCredentialRecoveryView : UserControl
{
    public DatabaseCredentialRecoveryView()
    {
        this.DataContext = new DatabaseCredentialRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseCredentialRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}