namespace AtlasOps.Features.Messaging.MessageSubscriptionMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageSubscriptionMonitoringView : UserControl
{
    public MessageSubscriptionMonitoringView()
    {
        this.DataContext = new MessageSubscriptionMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageSubscriptionMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}