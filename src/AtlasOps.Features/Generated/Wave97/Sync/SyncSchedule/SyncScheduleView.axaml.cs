namespace AtlasOps.Features.Sync.SyncSchedule;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class SyncScheduleView : UserControl
{
    public SyncScheduleView()
    {
        this.DataContext = new SyncScheduleViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is SyncScheduleViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}