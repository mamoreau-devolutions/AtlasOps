namespace AtlasOps.Features.Messaging.MessageSchemaOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageSchemaOptimizationView : UserControl
{
    public MessageSchemaOptimizationView()
    {
        this.DataContext = new MessageSchemaOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageSchemaOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}