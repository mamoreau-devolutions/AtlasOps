namespace AtlasOps.Features.Editor.DocumentSession;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DocumentSessionView : UserControl
{
    public DocumentSessionView()
    {
        this.DataContext = new DocumentSessionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DocumentSessionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}