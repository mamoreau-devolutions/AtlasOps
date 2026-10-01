namespace AtlasOps.Features.Storage.StorageReplicationGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageReplicationGovernanceView : UserControl
{
    public StorageReplicationGovernanceView()
    {
        this.DataContext = new StorageReplicationGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageReplicationGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}