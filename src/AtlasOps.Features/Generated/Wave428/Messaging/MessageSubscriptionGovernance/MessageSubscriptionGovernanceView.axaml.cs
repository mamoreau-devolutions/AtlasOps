namespace AtlasOps.Features.Messaging.MessageSubscriptionGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageSubscriptionGovernanceView : UserControl
{
    public MessageSubscriptionGovernanceView()
    {
        this.DataContext = new MessageSubscriptionGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageSubscriptionGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}