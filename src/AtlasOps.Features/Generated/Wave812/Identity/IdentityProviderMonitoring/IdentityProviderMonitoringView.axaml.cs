namespace AtlasOps.Features.Identity.IdentityProviderMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityProviderMonitoringView : UserControl
{
    public IdentityProviderMonitoringView()
    {
        this.DataContext = new IdentityProviderMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityProviderMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}