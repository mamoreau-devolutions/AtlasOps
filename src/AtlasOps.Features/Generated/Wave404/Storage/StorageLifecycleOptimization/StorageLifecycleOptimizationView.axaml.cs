namespace AtlasOps.Features.Storage.StorageLifecycleOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageLifecycleOptimizationView : UserControl
{
    public StorageLifecycleOptimizationView()
    {
        this.DataContext = new StorageLifecycleOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageLifecycleOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}