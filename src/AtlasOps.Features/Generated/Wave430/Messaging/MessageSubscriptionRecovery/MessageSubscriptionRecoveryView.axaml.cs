namespace AtlasOps.Features.Messaging.MessageSubscriptionRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageSubscriptionRecoveryView : UserControl
{
    public MessageSubscriptionRecoveryView()
    {
        this.DataContext = new MessageSubscriptionRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageSubscriptionRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}