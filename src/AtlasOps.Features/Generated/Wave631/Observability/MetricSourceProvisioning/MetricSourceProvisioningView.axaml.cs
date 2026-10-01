namespace AtlasOps.Features.Observability.MetricSourceProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class MetricSourceProvisioningView : UserControl
{
    public MetricSourceProvisioningView()
    {
        this.DataContext = new MetricSourceProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is MetricSourceProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}