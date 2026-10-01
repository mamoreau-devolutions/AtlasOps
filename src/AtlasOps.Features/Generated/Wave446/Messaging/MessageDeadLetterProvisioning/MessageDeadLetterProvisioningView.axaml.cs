namespace AtlasOps.Features.Messaging.MessageDeadLetterProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageDeadLetterProvisioningView : UserControl
{
    public MessageDeadLetterProvisioningView()
    {
        this.DataContext = new MessageDeadLetterProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageDeadLetterProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}