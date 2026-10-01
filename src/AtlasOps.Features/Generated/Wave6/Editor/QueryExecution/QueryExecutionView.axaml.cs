namespace AtlasOps.Features.Editor.QueryExecution;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class QueryExecutionView : UserControl
{
    public QueryExecutionView()
    {
        this.DataContext = new QueryExecutionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is QueryExecutionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}