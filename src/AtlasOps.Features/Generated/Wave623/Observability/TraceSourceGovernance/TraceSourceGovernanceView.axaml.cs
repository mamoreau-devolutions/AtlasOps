namespace AtlasOps.Features.Observability.TraceSourceGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TraceSourceGovernanceView : UserControl
{
    public TraceSourceGovernanceView()
    {
        this.DataContext = new TraceSourceGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TraceSourceGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}