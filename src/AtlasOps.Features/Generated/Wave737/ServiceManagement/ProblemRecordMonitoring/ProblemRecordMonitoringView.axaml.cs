namespace AtlasOps.Features.ServiceManagement.ProblemRecordMonitoring;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ProblemRecordMonitoringView : UserControl
{
    public ProblemRecordMonitoringView()
    {
        this.DataContext = new ProblemRecordMonitoringViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ProblemRecordMonitoringViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}