namespace AtlasOps.Features.Storage.StorageTransferGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageTransferGovernanceView : UserControl
{
    public StorageTransferGovernanceView()
    {
        this.DataContext = new StorageTransferGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageTransferGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}