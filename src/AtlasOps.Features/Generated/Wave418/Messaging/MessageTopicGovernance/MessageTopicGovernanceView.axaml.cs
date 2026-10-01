namespace AtlasOps.Features.Messaging.MessageTopicGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageTopicGovernanceView : UserControl
{
    public MessageTopicGovernanceView()
    {
        this.DataContext = new MessageTopicGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageTopicGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}