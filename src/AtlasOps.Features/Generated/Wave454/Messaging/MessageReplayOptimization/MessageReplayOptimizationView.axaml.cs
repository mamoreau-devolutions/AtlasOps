namespace AtlasOps.Features.Messaging.MessageReplayOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageReplayOptimizationView : UserControl
{
    public MessageReplayOptimizationView()
    {
        this.DataContext = new MessageReplayOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageReplayOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}