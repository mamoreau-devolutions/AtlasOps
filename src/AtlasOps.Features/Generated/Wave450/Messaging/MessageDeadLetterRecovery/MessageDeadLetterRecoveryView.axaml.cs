namespace AtlasOps.Features.Messaging.MessageDeadLetterRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageDeadLetterRecoveryView : UserControl
{
    public MessageDeadLetterRecoveryView()
    {
        this.DataContext = new MessageDeadLetterRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageDeadLetterRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}