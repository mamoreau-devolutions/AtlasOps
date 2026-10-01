namespace AtlasOps.Features.Storage.StorageArchiveGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageArchiveGovernanceView : UserControl
{
    public StorageArchiveGovernanceView()
    {
        this.DataContext = new StorageArchiveGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageArchiveGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}