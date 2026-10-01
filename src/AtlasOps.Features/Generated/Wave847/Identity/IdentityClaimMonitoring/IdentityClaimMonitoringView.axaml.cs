namespace AtlasOps.Features.Identity.IdentityClaimMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityClaimMonitoringView : UserControl
{
    public IdentityClaimMonitoringView()
    {
        this.DataContext = new IdentityClaimMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityClaimMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}