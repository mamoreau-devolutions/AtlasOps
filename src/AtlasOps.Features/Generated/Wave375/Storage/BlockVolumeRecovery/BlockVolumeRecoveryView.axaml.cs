namespace AtlasOps.Features.Storage.BlockVolumeRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BlockVolumeRecoveryView : UserControl
{
    public BlockVolumeRecoveryView()
    {
        this.DataContext = new BlockVolumeRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BlockVolumeRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}