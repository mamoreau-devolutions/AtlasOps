namespace AtlasOps.Features.Platform.ExtensionMarketplace;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ExtensionMarketplaceView : UserControl
{
    public ExtensionMarketplaceView()
    {
        this.DataContext = new ExtensionMarketplaceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ExtensionMarketplaceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}