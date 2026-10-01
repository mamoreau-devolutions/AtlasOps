namespace AtlasOps.Features.Messaging.MessageQueueGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageQueueGovernanceView : UserControl
{
    public MessageQueueGovernanceView()
    {
        this.DataContext = new MessageQueueGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageQueueGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}