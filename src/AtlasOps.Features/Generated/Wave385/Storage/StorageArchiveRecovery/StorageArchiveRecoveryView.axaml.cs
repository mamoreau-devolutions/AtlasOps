namespace AtlasOps.Features.Storage.StorageArchiveRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageArchiveRecoveryView : UserControl
{
    public StorageArchiveRecoveryView()
    {
        this.DataContext = new StorageArchiveRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageArchiveRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}