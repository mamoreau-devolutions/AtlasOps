namespace AtlasOps.Features.Storage.StorageQuotaGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageQuotaGovernanceView : UserControl
{
    public StorageQuotaGovernanceView()
    {
        this.DataContext = new StorageQuotaGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageQuotaGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}