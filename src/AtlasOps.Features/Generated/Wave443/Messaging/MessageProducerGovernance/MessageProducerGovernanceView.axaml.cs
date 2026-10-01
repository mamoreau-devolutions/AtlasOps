namespace AtlasOps.Features.Messaging.MessageProducerGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageProducerGovernanceView : UserControl
{
    public MessageProducerGovernanceView()
    {
        this.DataContext = new MessageProducerGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageProducerGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}