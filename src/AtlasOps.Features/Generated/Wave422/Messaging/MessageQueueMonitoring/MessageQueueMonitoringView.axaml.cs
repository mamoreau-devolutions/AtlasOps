namespace AtlasOps.Features.Messaging.MessageQueueMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageQueueMonitoringView : UserControl
{
    public MessageQueueMonitoringView()
    {
        this.DataContext = new MessageQueueMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageQueueMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}