namespace AtlasOps.Features.Inventory.AssetLifecycle;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class AssetLifecycleView : UserControl
{
    public AssetLifecycleView()
    {
        this.DataContext = new AssetLifecycleViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is AssetLifecycleViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}