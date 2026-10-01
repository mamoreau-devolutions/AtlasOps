namespace AtlasOps.Features.Editor.QueryParameter;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class QueryParameterView : UserControl
{
    public QueryParameterView()
    {
        this.DataContext = new QueryParameterViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is QueryParameterViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}