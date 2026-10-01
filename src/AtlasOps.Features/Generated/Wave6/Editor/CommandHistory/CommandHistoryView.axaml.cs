namespace AtlasOps.Features.Editor.CommandHistory;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CommandHistoryView : UserControl
{
    public CommandHistoryView()
    {
        this.DataContext = new CommandHistoryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CommandHistoryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}