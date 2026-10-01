namespace AtlasOps.Features.Sync.OfflineQueue;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class OfflineQueueView : UserControl
{
    public OfflineQueueView()
    {
        this.DataContext = new OfflineQueueViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is OfflineQueueViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}