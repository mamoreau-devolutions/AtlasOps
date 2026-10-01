namespace AtlasOps.Features.Security.SecurityPatchMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SecurityPatchMonitoringView : UserControl
{
    public SecurityPatchMonitoringView()
    {
        this.DataContext = new SecurityPatchMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SecurityPatchMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}