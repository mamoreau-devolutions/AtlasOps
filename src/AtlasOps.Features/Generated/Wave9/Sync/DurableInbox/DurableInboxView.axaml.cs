namespace AtlasOps.Features.Sync.DurableInbox;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DurableInboxView : UserControl
{
    public DurableInboxView()
    {
        this.DataContext = new DurableInboxViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DurableInboxViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}