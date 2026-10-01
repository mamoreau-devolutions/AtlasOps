namespace AtlasOps.Features.Messaging.MessageConsumerGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageConsumerGovernanceView : UserControl
{
    public MessageConsumerGovernanceView()
    {
        this.DataContext = new MessageConsumerGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageConsumerGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}