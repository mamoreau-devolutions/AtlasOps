namespace AtlasOps.Features.Incidents.IncidentNotification;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IncidentNotificationView : UserControl
{
    public IncidentNotificationView()
    {
        this.DataContext = new IncidentNotificationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IncidentNotificationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}