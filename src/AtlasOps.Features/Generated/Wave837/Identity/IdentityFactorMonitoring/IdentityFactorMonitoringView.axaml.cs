namespace AtlasOps.Features.Identity.IdentityFactorMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class IdentityFactorMonitoringView : UserControl
{
    public IdentityFactorMonitoringView()
    {
        this.DataContext = new IdentityFactorMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is IdentityFactorMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}