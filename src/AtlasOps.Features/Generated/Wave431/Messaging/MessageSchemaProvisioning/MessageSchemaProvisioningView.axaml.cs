namespace AtlasOps.Features.Messaging.MessageSchemaProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageSchemaProvisioningView : UserControl
{
    public MessageSchemaProvisioningView()
    {
        this.DataContext = new MessageSchemaProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageSchemaProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}