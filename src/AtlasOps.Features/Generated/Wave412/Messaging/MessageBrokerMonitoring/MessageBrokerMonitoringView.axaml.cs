namespace AtlasOps.Features.Messaging.MessageBrokerMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageBrokerMonitoringView : UserControl
{
    public MessageBrokerMonitoringView()
    {
        this.DataContext = new MessageBrokerMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageBrokerMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}