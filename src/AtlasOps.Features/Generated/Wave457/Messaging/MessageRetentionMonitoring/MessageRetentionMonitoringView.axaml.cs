namespace AtlasOps.Features.Messaging.MessageRetentionMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageRetentionMonitoringView : UserControl
{
    public MessageRetentionMonitoringView()
    {
        this.DataContext = new MessageRetentionMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageRetentionMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}