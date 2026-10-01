namespace AtlasOps.Features.Automation.ScheduledRunbook;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ScheduledRunbookView : UserControl
{
    public ScheduledRunbookView()
    {
        this.DataContext = new ScheduledRunbookViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ScheduledRunbookViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}