namespace AtlasOps.Features.Messaging.MessageReplayProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageReplayProvisioningView : UserControl
{
    public MessageReplayProvisioningView()
    {
        this.DataContext = new MessageReplayProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageReplayProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}