namespace AtlasOps.Features.Editor.QueryPlan;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class QueryPlanView : UserControl
{
    public QueryPlanView()
    {
        this.DataContext = new QueryPlanViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is QueryPlanViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}