namespace AtlasOps.Features.Storage.StorageArchiveOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageArchiveOptimizationView : UserControl
{
    public StorageArchiveOptimizationView()
    {
        this.DataContext = new StorageArchiveOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageArchiveOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}