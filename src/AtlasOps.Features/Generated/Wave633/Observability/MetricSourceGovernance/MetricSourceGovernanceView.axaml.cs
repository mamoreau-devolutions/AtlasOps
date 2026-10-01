namespace AtlasOps.Features.Observability.MetricSourceGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MetricSourceGovernanceView : UserControl
{
    public MetricSourceGovernanceView()
    {
        this.DataContext = new MetricSourceGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MetricSourceGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}