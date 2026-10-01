namespace AtlasOps.Features.Messaging.MessageTopicProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageTopicProvisioningView : UserControl
{
    public MessageTopicProvisioningView()
    {
        this.DataContext = new MessageTopicProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageTopicProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}