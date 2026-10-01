namespace AtlasOps.Features.Mobile.MobileTelemetryOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MobileTelemetryOptimizationView : UserControl
{
    public MobileTelemetryOptimizationView()
    {
        this.DataContext = new MobileTelemetryOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MobileTelemetryOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}