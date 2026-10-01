namespace AtlasOps.Features.Sync.SyncBatch;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SyncBatchView : UserControl
{
    public SyncBatchView()
    {
        this.DataContext = new SyncBatchViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SyncBatchViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}