namespace AtlasOps.Features.Messaging.MessageRetentionProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageRetentionProvisioningView : UserControl
{
    public MessageRetentionProvisioningView()
    {
        this.DataContext = new MessageRetentionProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageRetentionProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}