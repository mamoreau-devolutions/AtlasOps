namespace AtlasOps.Features.Incidents.RemediationTracking;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class RemediationTrackingView : UserControl
{
    public RemediationTrackingView()
    {
        this.DataContext = new RemediationTrackingViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is RemediationTrackingViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}