namespace AtlasOps.Features.Messaging.MessageQueueRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageQueueRecoveryView : UserControl
{
    public MessageQueueRecoveryView()
    {
        this.DataContext = new MessageQueueRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageQueueRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}