namespace AtlasOps.Features.Database.DatabaseQueryRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DatabaseQueryRecoveryView : UserControl
{
    public DatabaseQueryRecoveryView()
    {
        this.DataContext = new DatabaseQueryRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DatabaseQueryRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}