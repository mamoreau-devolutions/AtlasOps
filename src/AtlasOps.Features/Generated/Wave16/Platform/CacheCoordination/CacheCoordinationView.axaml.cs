namespace AtlasOps.Features.Platform.CacheCoordination;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CacheCoordinationView : UserControl
{
    public CacheCoordinationView()
    {
        this.DataContext = new CacheCoordinationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CacheCoordinationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}