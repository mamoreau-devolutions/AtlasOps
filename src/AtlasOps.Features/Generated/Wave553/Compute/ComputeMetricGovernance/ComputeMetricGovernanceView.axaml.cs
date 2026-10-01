namespace AtlasOps.Features.Compute.ComputeMetricGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeMetricGovernanceView : UserControl
{
    public ComputeMetricGovernanceView()
    {
        this.DataContext = new ComputeMetricGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeMetricGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}