namespace AtlasOps.Features.Identity.IdentitySessionMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentitySessionMonitoringView : UserControl
{
    public IdentitySessionMonitoringView()
    {
        this.DataContext = new IdentitySessionMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentitySessionMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}