namespace AtlasOps.Features.Connections.HttpEndpoint;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class HttpEndpointView : UserControl
{
    public HttpEndpointView()
    {
        this.DataContext = new HttpEndpointViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is HttpEndpointViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}