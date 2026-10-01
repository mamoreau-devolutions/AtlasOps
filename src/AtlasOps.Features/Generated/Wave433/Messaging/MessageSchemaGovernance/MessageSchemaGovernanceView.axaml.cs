namespace AtlasOps.Features.Messaging.MessageSchemaGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageSchemaGovernanceView : UserControl
{
    public MessageSchemaGovernanceView()
    {
        this.DataContext = new MessageSchemaGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageSchemaGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}