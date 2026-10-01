namespace AtlasOps.Features.Editor.ResultComparison;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ResultComparisonView : UserControl
{
    public ResultComparisonView()
    {
        this.DataContext = new ResultComparisonViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ResultComparisonViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}