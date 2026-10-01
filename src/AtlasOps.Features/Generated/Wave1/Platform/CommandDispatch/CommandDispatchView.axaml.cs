namespace AtlasOps.Features.Platform.CommandDispatch;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CommandDispatchView : UserControl
{
    public CommandDispatchView()
    {
        this.DataContext = new CommandDispatchViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CommandDispatchViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}