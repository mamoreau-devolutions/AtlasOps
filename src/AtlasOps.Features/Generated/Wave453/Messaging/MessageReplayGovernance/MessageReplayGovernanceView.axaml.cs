namespace AtlasOps.Features.Messaging.MessageReplayGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageReplayGovernanceView : UserControl
{
    public MessageReplayGovernanceView()
    {
        this.DataContext = new MessageReplayGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageReplayGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}