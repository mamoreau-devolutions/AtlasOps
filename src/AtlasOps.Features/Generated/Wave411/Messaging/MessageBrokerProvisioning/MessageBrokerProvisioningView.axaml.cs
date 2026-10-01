namespace AtlasOps.Features.Messaging.MessageBrokerProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageBrokerProvisioningView : UserControl
{
    public MessageBrokerProvisioningView()
    {
        this.DataContext = new MessageBrokerProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageBrokerProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}