namespace AtlasOps.Features.Compute.ComputeScheduleGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeScheduleGovernanceView : UserControl
{
    public ComputeScheduleGovernanceView()
    {
        this.DataContext = new ComputeScheduleGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeScheduleGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}