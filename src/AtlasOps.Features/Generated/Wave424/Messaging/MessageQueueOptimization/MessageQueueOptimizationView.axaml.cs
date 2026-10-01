namespace AtlasOps.Features.Messaging.MessageQueueOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageQueueOptimizationView : UserControl
{
    public MessageQueueOptimizationView()
    {
        this.DataContext = new MessageQueueOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageQueueOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}