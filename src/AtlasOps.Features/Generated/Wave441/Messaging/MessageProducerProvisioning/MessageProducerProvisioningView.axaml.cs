namespace AtlasOps.Features.Messaging.MessageProducerProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageProducerProvisioningView : UserControl
{
    public MessageProducerProvisioningView()
    {
        this.DataContext = new MessageProducerProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageProducerProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}