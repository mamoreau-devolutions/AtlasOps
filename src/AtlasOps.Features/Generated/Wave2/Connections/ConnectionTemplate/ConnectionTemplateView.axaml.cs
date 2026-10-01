namespace AtlasOps.Features.Connections.ConnectionTemplate;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ConnectionTemplateView : UserControl
{
    public ConnectionTemplateView()
    {
        this.DataContext = new ConnectionTemplateViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ConnectionTemplateViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}