namespace AtlasOps.Features.Storage.BlockVolumeOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BlockVolumeOptimizationView : UserControl
{
    public BlockVolumeOptimizationView()
    {
        this.DataContext = new BlockVolumeOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BlockVolumeOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}