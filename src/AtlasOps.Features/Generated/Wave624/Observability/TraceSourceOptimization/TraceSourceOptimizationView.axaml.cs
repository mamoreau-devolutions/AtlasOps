namespace AtlasOps.Features.Observability.TraceSourceOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class TraceSourceOptimizationView : UserControl
{
    public TraceSourceOptimizationView()
    {
        this.DataContext = new TraceSourceOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is TraceSourceOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}