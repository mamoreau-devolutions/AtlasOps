namespace AtlasOps.Features.Messaging.MessageReplayRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageReplayRecoveryView : UserControl
{
    public MessageReplayRecoveryView()
    {
        this.DataContext = new MessageReplayRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageReplayRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}