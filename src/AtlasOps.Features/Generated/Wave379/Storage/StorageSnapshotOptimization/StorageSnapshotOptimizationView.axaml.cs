namespace AtlasOps.Features.Storage.StorageSnapshotOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageSnapshotOptimizationView : UserControl
{
    public StorageSnapshotOptimizationView()
    {
        this.DataContext = new StorageSnapshotOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageSnapshotOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}