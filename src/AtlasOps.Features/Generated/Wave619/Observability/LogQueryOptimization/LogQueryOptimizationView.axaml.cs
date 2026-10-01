namespace AtlasOps.Features.Observability.LogQueryOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class LogQueryOptimizationView : UserControl
{
    public LogQueryOptimizationView()
    {
        this.DataContext = new LogQueryOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is LogQueryOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}