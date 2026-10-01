namespace AtlasOps.Features.Messaging.MessageRetentionRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageRetentionRecoveryView : UserControl
{
    public MessageRetentionRecoveryView()
    {
        this.DataContext = new MessageRetentionRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageRetentionRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}