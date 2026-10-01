namespace AtlasOps.Features.Identity.IdentityLifecycleMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityLifecycleMonitoringView : UserControl
{
    public IdentityLifecycleMonitoringView()
    {
        this.DataContext = new IdentityLifecycleMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityLifecycleMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}