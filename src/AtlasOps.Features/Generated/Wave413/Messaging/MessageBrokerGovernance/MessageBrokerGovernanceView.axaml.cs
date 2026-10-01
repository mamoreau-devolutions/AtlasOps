namespace AtlasOps.Features.Messaging.MessageBrokerGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageBrokerGovernanceView : UserControl
{
    public MessageBrokerGovernanceView()
    {
        this.DataContext = new MessageBrokerGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageBrokerGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}