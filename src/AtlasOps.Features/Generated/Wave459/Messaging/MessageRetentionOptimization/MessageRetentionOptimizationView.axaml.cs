namespace AtlasOps.Features.Messaging.MessageRetentionOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageRetentionOptimizationView : UserControl
{
    public MessageRetentionOptimizationView()
    {
        this.DataContext = new MessageRetentionOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageRetentionOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}