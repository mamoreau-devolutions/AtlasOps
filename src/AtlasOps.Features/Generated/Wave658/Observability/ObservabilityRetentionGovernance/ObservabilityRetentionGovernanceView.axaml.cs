namespace AtlasOps.Features.Observability.ObservabilityRetentionGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilityRetentionGovernanceView : UserControl
{
    public ObservabilityRetentionGovernanceView()
    {
        this.DataContext = new ObservabilityRetentionGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilityRetentionGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}