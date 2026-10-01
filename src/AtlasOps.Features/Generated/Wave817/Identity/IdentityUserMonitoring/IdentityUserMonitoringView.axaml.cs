namespace AtlasOps.Features.Identity.IdentityUserMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityUserMonitoringView : UserControl
{
    public IdentityUserMonitoringView()
    {
        this.DataContext = new IdentityUserMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityUserMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}