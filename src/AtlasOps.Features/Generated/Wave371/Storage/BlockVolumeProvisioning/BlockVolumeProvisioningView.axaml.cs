namespace AtlasOps.Features.Storage.BlockVolumeProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BlockVolumeProvisioningView : UserControl
{
    public BlockVolumeProvisioningView()
    {
        this.DataContext = new BlockVolumeProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BlockVolumeProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}