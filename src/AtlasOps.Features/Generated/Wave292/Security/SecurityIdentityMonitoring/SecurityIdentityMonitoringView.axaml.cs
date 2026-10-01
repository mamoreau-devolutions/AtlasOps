namespace AtlasOps.Features.Security.SecurityIdentityMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityIdentityMonitoringView : UserControl
{
    public SecurityIdentityMonitoringView()
    {
        this.DataContext = new SecurityIdentityMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityIdentityMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}