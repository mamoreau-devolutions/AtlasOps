namespace AtlasOps.Features.Messaging.MessageDeadLetterMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageDeadLetterMonitoringView : UserControl
{
    public MessageDeadLetterMonitoringView()
    {
        this.DataContext = new MessageDeadLetterMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageDeadLetterMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}