namespace AtlasOps.Features.Storage.StorageTransferOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageTransferOptimizationView : UserControl
{
    public StorageTransferOptimizationView()
    {
        this.DataContext = new StorageTransferOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageTransferOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}