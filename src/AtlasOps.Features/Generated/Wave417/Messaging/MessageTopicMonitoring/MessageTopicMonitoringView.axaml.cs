namespace AtlasOps.Features.Messaging.MessageTopicMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageTopicMonitoringView : UserControl
{
    public MessageTopicMonitoringView()
    {
        this.DataContext = new MessageTopicMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageTopicMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}