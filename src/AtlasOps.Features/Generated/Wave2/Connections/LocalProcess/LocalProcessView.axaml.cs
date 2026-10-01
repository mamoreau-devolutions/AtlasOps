namespace AtlasOps.Features.Connections.LocalProcess;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LocalProcessView : UserControl
{
    public LocalProcessView()
    {
        this.DataContext = new LocalProcessViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LocalProcessViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}