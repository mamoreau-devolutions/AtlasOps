namespace AtlasOps.Features.Editor.NotebookDocument;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NotebookDocumentView : UserControl
{
    public NotebookDocumentView()
    {
        this.DataContext = new NotebookDocumentViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NotebookDocumentViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}