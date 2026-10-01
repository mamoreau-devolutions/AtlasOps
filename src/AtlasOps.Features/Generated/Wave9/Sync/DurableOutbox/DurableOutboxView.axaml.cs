namespace AtlasOps.Features.Sync.DurableOutbox;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DurableOutboxView : UserControl
{
    public DurableOutboxView()
    {
        this.DataContext = new DurableOutboxViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DurableOutboxViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}