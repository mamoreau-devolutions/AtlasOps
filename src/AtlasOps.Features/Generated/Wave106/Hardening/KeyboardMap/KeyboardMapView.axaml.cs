namespace AtlasOps.Features.Hardening.KeyboardMap;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class KeyboardMapView : UserControl
{
    public KeyboardMapView()
    {
        this.DataContext = new KeyboardMapViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is KeyboardMapViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}