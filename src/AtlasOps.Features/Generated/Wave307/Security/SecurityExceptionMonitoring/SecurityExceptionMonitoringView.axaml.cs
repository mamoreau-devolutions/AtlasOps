namespace AtlasOps.Features.Security.SecurityExceptionMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityExceptionMonitoringView : UserControl
{
    public SecurityExceptionMonitoringView()
    {
        this.DataContext = new SecurityExceptionMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityExceptionMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}