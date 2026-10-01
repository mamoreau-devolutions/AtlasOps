namespace AtlasOps.Features.Identity.IdentityApplicationMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityApplicationMonitoringView : UserControl
{
    public IdentityApplicationMonitoringView()
    {
        this.DataContext = new IdentityApplicationMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityApplicationMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}