namespace AtlasOps.Features.FinOps.SavingsPlanMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SavingsPlanMonitoringView : UserControl
{
    public SavingsPlanMonitoringView()
    {
        this.DataContext = new SavingsPlanMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SavingsPlanMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}