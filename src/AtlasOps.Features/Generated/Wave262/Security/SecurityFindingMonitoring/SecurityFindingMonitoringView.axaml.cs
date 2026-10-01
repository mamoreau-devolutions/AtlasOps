namespace AtlasOps.Features.Security.SecurityFindingMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityFindingMonitoringView : UserControl
{
    public SecurityFindingMonitoringView()
    {
        this.DataContext = new SecurityFindingMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityFindingMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}