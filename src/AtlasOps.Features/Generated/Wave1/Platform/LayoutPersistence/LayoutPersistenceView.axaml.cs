namespace AtlasOps.Features.Platform.LayoutPersistence;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LayoutPersistenceView : UserControl
{
    public LayoutPersistenceView()
    {
        this.DataContext = new LayoutPersistenceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LayoutPersistenceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}