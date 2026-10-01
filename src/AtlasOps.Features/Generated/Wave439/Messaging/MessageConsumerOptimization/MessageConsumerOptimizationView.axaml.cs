namespace AtlasOps.Features.Messaging.MessageConsumerOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageConsumerOptimizationView : UserControl
{
    public MessageConsumerOptimizationView()
    {
        this.DataContext = new MessageConsumerOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageConsumerOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}