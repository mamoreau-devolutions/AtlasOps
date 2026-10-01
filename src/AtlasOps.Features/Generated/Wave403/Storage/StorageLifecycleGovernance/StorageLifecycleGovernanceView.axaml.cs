namespace AtlasOps.Features.Storage.StorageLifecycleGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageLifecycleGovernanceView : UserControl
{
    public StorageLifecycleGovernanceView()
    {
        this.DataContext = new StorageLifecycleGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageLifecycleGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}