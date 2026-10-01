namespace AtlasOps.Features.Storage.StorageEncryptionOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageEncryptionOptimizationView : UserControl
{
    public StorageEncryptionOptimizationView()
    {
        this.DataContext = new StorageEncryptionOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageEncryptionOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}