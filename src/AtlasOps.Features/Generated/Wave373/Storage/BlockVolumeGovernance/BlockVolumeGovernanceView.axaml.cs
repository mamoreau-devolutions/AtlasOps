namespace AtlasOps.Features.Storage.BlockVolumeGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class BlockVolumeGovernanceView : UserControl
{
    public BlockVolumeGovernanceView()
    {
        this.DataContext = new BlockVolumeGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is BlockVolumeGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}