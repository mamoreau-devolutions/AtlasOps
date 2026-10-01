namespace AtlasOps.Features.Messaging.MessageProducerMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageProducerMonitoringView : UserControl
{
    public MessageProducerMonitoringView()
    {
        this.DataContext = new MessageProducerMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageProducerMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}