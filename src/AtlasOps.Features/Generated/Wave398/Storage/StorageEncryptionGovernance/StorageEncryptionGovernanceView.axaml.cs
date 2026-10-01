namespace AtlasOps.Features.Storage.StorageEncryptionGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageEncryptionGovernanceView : UserControl
{
    public StorageEncryptionGovernanceView()
    {
        this.DataContext = new StorageEncryptionGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageEncryptionGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}