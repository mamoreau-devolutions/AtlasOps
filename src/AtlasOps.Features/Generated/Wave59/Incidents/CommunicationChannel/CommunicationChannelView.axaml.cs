namespace AtlasOps.Features.Incidents.CommunicationChannel;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class CommunicationChannelView : UserControl
{
    public CommunicationChannelView()
    {
        this.DataContext = new CommunicationChannelViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is CommunicationChannelViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}