namespace AtlasOps.Features.Messaging.MessageQueueProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageQueueProvisioningView : UserControl
{
    public MessageQueueProvisioningView()
    {
        this.DataContext = new MessageQueueProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageQueueProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}