namespace AtlasOps.Features.Observability.ObservabilityExportGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilityExportGovernanceView : UserControl
{
    public ObservabilityExportGovernanceView()
    {
        this.DataContext = new ObservabilityExportGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilityExportGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}