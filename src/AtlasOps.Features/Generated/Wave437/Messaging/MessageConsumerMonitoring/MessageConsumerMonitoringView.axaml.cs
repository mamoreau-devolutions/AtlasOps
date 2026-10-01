namespace AtlasOps.Features.Messaging.MessageConsumerMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageConsumerMonitoringView : UserControl
{
    public MessageConsumerMonitoringView()
    {
        this.DataContext = new MessageConsumerMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageConsumerMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}