namespace AtlasOps.Features.Compute.ComputeMetricProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ComputeMetricProvisioningView : UserControl
{
    public ComputeMetricProvisioningView()
    {
        this.DataContext = new ComputeMetricProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ComputeMetricProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}