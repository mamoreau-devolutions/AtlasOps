namespace AtlasOps.Features.Identity.IdentityAuditMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityAuditMonitoringView : UserControl
{
    public IdentityAuditMonitoringView()
    {
        this.DataContext = new IdentityAuditMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityAuditMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}