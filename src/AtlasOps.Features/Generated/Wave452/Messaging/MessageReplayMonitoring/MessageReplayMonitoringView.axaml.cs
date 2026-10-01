namespace AtlasOps.Features.Messaging.MessageReplayMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageReplayMonitoringView : UserControl
{
    public MessageReplayMonitoringView()
    {
        this.DataContext = new MessageReplayMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageReplayMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}