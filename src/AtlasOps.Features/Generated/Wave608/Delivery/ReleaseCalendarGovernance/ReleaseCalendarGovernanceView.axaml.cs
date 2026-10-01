namespace AtlasOps.Features.Delivery.ReleaseCalendarGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ReleaseCalendarGovernanceView : UserControl
{
    public ReleaseCalendarGovernanceView()
    {
        this.DataContext = new ReleaseCalendarGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ReleaseCalendarGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}