namespace AtlasOps.Features.Storage.StorageLifecycleRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageLifecycleRecoveryView : UserControl
{
    public StorageLifecycleRecoveryView()
    {
        this.DataContext = new StorageLifecycleRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageLifecycleRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}