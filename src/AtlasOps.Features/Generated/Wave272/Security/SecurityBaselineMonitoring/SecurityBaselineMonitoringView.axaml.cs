namespace AtlasOps.Features.Security.SecurityBaselineMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityBaselineMonitoringView : UserControl
{
    public SecurityBaselineMonitoringView()
    {
        this.DataContext = new SecurityBaselineMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityBaselineMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}