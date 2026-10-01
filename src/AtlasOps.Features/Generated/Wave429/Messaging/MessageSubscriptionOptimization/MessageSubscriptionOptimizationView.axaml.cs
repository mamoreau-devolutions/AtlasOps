namespace AtlasOps.Features.Messaging.MessageSubscriptionOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageSubscriptionOptimizationView : UserControl
{
    public MessageSubscriptionOptimizationView()
    {
        this.DataContext = new MessageSubscriptionOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageSubscriptionOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}