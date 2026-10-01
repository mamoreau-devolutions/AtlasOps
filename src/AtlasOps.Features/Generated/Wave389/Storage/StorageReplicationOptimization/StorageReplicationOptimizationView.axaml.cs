namespace AtlasOps.Features.Storage.StorageReplicationOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageReplicationOptimizationView : UserControl
{
    public StorageReplicationOptimizationView()
    {
        this.DataContext = new StorageReplicationOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageReplicationOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}