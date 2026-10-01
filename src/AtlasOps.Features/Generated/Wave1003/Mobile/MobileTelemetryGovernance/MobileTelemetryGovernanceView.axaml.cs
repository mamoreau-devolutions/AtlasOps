namespace AtlasOps.Features.Mobile.MobileTelemetryGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileTelemetryGovernanceView : UserControl
{
    public MobileTelemetryGovernanceView()
    {
        this.DataContext = new MobileTelemetryGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileTelemetryGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}