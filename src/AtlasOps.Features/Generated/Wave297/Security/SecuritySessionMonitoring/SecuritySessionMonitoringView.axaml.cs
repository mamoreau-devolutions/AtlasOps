namespace AtlasOps.Features.Security.SecuritySessionMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecuritySessionMonitoringView : UserControl
{
    public SecuritySessionMonitoringView()
    {
        this.DataContext = new SecuritySessionMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecuritySessionMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}