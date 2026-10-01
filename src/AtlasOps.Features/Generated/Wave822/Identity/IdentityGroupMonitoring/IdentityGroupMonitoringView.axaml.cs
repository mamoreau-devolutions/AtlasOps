namespace AtlasOps.Features.Identity.IdentityGroupMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityGroupMonitoringView : UserControl
{
    public IdentityGroupMonitoringView()
    {
        this.DataContext = new IdentityGroupMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityGroupMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}