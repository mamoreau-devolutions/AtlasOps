namespace AtlasOps.Features.Observability.TraceSpanGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TraceSpanGovernanceView : UserControl
{
    public TraceSpanGovernanceView()
    {
        this.DataContext = new TraceSpanGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TraceSpanGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}