namespace AtlasOps.Features.Database.DatabaseIndexRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseIndexRecoveryView : UserControl
{
    public DatabaseIndexRecoveryView()
    {
        this.DataContext = new DatabaseIndexRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseIndexRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}