namespace AtlasOps.Features.Compute.ComputeMetricRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeMetricRecoveryView : UserControl
{
    public ComputeMetricRecoveryView()
    {
        this.DataContext = new ComputeMetricRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeMetricRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}