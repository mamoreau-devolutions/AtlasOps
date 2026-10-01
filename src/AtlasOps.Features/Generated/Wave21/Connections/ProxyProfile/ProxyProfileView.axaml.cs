namespace AtlasOps.Features.Connections.ProxyProfile;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ProxyProfileView : UserControl
{
    public ProxyProfileView()
    {
        this.DataContext = new ProxyProfileViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ProxyProfileViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}