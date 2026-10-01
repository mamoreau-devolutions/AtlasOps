namespace AtlasOps.Features.Analytics.ReportSchedule;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReportScheduleView : UserControl
{
    public ReportScheduleView()
    {
        this.DataContext = new ReportScheduleViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReportScheduleViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}