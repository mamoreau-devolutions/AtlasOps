namespace AtlasOps.Features.Incidents.OnCallSchedule;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class OnCallScheduleView : UserControl
{
    public OnCallScheduleView()
    {
        this.DataContext = new OnCallScheduleViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is OnCallScheduleViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}