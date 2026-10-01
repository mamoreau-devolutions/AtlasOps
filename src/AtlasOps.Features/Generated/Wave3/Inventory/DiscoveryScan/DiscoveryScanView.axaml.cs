namespace AtlasOps.Features.Inventory.DiscoveryScan;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class DiscoveryScanView : UserControl
{
    public DiscoveryScanView()
    {
        this.DataContext = new DiscoveryScanViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is DiscoveryScanViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}