namespace AtlasOps.Features.Messaging.MessageTopicOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageTopicOptimizationView : UserControl
{
    public MessageTopicOptimizationView()
    {
        this.DataContext = new MessageTopicOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageTopicOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}