namespace AtlasOps.Features.Platform.ThemeComposition;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ThemeCompositionView : UserControl
{
    public ThemeCompositionView()
    {
        this.DataContext = new ThemeCompositionViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ThemeCompositionViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}