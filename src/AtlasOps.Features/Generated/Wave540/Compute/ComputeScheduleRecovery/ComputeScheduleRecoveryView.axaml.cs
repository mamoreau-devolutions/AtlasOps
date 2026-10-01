namespace AtlasOps.Features.Compute.ComputeScheduleRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeScheduleRecoveryView : UserControl
{
    public ComputeScheduleRecoveryView()
    {
        this.DataContext = new ComputeScheduleRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeScheduleRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}