namespace AtlasOps.Features.Messaging.MessageDeadLetterOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageDeadLetterOptimizationView : UserControl
{
    public MessageDeadLetterOptimizationView()
    {
        this.DataContext = new MessageDeadLetterOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageDeadLetterOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}