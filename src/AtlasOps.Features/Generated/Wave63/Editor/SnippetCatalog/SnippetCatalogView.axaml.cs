namespace AtlasOps.Features.Editor.SnippetCatalog;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SnippetCatalogView : UserControl
{
    public SnippetCatalogView()
    {
        this.DataContext = new SnippetCatalogViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SnippetCatalogViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}