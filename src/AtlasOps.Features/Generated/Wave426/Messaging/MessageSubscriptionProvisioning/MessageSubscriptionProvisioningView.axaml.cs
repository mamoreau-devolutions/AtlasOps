namespace AtlasOps.Features.Messaging.MessageSubscriptionProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageSubscriptionProvisioningView : UserControl
{
    public MessageSubscriptionProvisioningView()
    {
        this.DataContext = new MessageSubscriptionProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageSubscriptionProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}