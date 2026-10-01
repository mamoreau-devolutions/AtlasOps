namespace AtlasOps.Features.Storage.StorageQuotaRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageQuotaRecoveryView : UserControl
{
    public StorageQuotaRecoveryView()
    {
        this.DataContext = new StorageQuotaRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageQuotaRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}