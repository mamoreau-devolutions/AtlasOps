namespace AtlasOps.Features.Messaging.MessageConsumerRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageConsumerRecoveryView : UserControl
{
    public MessageConsumerRecoveryView()
    {
        this.DataContext = new MessageConsumerRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageConsumerRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}