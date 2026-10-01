namespace AtlasOps.Features.Messaging.MessageDeadLetterGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageDeadLetterGovernanceView : UserControl
{
    public MessageDeadLetterGovernanceView()
    {
        this.DataContext = new MessageDeadLetterGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageDeadLetterGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}