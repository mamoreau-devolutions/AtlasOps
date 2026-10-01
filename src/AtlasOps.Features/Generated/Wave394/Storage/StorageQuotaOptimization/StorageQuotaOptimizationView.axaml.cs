namespace AtlasOps.Features.Storage.StorageQuotaOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class StorageQuotaOptimizationView : UserControl
{
    public StorageQuotaOptimizationView()
    {
        this.DataContext = new StorageQuotaOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is StorageQuotaOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}