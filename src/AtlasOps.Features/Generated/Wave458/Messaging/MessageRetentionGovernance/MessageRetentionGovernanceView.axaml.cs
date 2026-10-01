namespace AtlasOps.Features.Messaging.MessageRetentionGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageRetentionGovernanceView : UserControl
{
    public MessageRetentionGovernanceView()
    {
        this.DataContext = new MessageRetentionGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageRetentionGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}