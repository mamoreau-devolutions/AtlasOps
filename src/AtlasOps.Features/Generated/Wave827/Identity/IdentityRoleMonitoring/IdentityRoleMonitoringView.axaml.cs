namespace AtlasOps.Features.Identity.IdentityRoleMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityRoleMonitoringView : UserControl
{
    public IdentityRoleMonitoringView()
    {
        this.DataContext = new IdentityRoleMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityRoleMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}