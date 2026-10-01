namespace AtlasOps.Features.Observability.ObservabilityExportProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ObservabilityExportProvisioningView : UserControl
{
    public ObservabilityExportProvisioningView()
    {
        this.DataContext = new ObservabilityExportProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ObservabilityExportProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}