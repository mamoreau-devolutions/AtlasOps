namespace AtlasOps.Features.Editor.QueryDefinition;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class QueryDefinitionView : UserControl
{
    public QueryDefinitionView()
    {
        this.DataContext = new QueryDefinitionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is QueryDefinitionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}