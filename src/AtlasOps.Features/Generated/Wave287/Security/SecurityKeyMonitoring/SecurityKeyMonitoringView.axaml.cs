namespace AtlasOps.Features.Security.SecurityKeyMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityKeyMonitoringView : UserControl
{
    public SecurityKeyMonitoringView()
    {
        this.DataContext = new SecurityKeyMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityKeyMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}