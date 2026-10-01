namespace AtlasOps.Features.Messaging.MessageSchemaMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MessageSchemaMonitoringView : UserControl
{
    public MessageSchemaMonitoringView()
    {
        this.DataContext = new MessageSchemaMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MessageSchemaMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}