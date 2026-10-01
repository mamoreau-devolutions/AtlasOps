namespace AtlasOps.Features.Observability.ObservabilityRetentionOptimization;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilityRetentionOptimizationView : UserControl
{
    public ObservabilityRetentionOptimizationView()
    {
        this.DataContext = new ObservabilityRetentionOptimizationViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilityRetentionOptimizationViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}