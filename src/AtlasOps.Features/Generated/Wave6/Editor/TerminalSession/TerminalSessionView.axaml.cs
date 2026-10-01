namespace AtlasOps.Features.Editor.TerminalSession;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TerminalSessionView : UserControl
{
    public TerminalSessionView()
    {
        this.DataContext = new TerminalSessionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TerminalSessionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}