namespace AtlasOps.Features.Messaging.MessageProducerOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageProducerOptimizationView : UserControl
{
    public MessageProducerOptimizationView()
    {
        this.DataContext = new MessageProducerOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageProducerOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}