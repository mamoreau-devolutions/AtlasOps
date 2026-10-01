namespace AtlasOps.Features.Messaging.MessageSchemaRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageSchemaRecoveryView : UserControl
{
    public MessageSchemaRecoveryView()
    {
        this.DataContext = new MessageSchemaRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageSchemaRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}