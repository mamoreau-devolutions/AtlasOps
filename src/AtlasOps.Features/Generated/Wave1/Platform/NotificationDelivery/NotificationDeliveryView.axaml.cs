namespace AtlasOps.Features.Platform.NotificationDelivery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class NotificationDeliveryView : UserControl
{
    public NotificationDeliveryView()
    {
        this.DataContext = new NotificationDeliveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is NotificationDeliveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}