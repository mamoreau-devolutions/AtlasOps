namespace AtlasOps.Features.ServiceManagement.ChangeRequestMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ChangeRequestMonitoringView : UserControl
{
    public ChangeRequestMonitoringView()
    {
        this.DataContext = new ChangeRequestMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ChangeRequestMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}