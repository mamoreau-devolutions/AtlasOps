namespace AtlasOps.Features.Messaging.MessageBrokerOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageBrokerOptimizationView : UserControl
{
    public MessageBrokerOptimizationView()
    {
        this.DataContext = new MessageBrokerOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageBrokerOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}