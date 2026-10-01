namespace AtlasOps.Features.Messaging.MessageBrokerRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageBrokerRecoveryView : UserControl
{
    public MessageBrokerRecoveryView()
    {
        this.DataContext = new MessageBrokerRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageBrokerRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}