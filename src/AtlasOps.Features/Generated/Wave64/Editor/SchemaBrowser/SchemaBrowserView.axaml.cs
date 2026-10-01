namespace AtlasOps.Features.Editor.SchemaBrowser;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SchemaBrowserView : UserControl
{
    public SchemaBrowserView()
    {
        this.DataContext = new SchemaBrowserViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SchemaBrowserViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}