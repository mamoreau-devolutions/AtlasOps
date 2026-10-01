namespace AtlasOps.Features.Messaging.MessageConsumerProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageConsumerProvisioningView : UserControl
{
    public MessageConsumerProvisioningView()
    {
        this.DataContext = new MessageConsumerProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageConsumerProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}