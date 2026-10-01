namespace AtlasOps.Features.Storage.StorageSnapshotGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageSnapshotGovernanceView : UserControl
{
    public StorageSnapshotGovernanceView()
    {
        this.DataContext = new StorageSnapshotGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageSnapshotGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}