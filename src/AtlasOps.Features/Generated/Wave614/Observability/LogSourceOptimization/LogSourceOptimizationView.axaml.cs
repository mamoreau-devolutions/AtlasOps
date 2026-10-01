namespace AtlasOps.Features.Observability.LogSourceOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LogSourceOptimizationView : UserControl
{
    public LogSourceOptimizationView()
    {
        this.DataContext = new LogSourceOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LogSourceOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}