namespace AtlasOps.Features.Platform.NavigationRouting;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NavigationRoutingView : UserControl
{
    public NavigationRoutingView()
    {
        this.DataContext = new NavigationRoutingViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NavigationRoutingViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}