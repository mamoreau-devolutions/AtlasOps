namespace AtlasOps.Features.Platform.FeatureDiscovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class FeatureDiscoveryView : UserControl
{
    public FeatureDiscoveryView()
    {
        this.DataContext = new FeatureDiscoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is FeatureDiscoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}