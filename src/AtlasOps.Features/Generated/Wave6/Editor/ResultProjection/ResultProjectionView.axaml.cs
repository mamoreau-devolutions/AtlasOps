namespace AtlasOps.Features.Editor.ResultProjection;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ResultProjectionView : UserControl
{
    public ResultProjectionView()
    {
        this.DataContext = new ResultProjectionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ResultProjectionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}