namespace AtlasOps.Features.Messaging.MessageTopicRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageTopicRecoveryView : UserControl
{
    public MessageTopicRecoveryView()
    {
        this.DataContext = new MessageTopicRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageTopicRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}