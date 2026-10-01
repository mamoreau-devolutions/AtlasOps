namespace AtlasOps.Features.Platform.LocalizationCatalog;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LocalizationCatalogView : UserControl
{
    public LocalizationCatalogView()
    {
        this.DataContext = new LocalizationCatalogViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LocalizationCatalogViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}