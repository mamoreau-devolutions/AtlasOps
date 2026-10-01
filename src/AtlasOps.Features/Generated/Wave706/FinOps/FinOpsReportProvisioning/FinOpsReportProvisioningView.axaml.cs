namespace AtlasOps.Features.FinOps.FinOpsReportProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class FinOpsReportProvisioningView : UserControl
{
    public FinOpsReportProvisioningView()
    {
        this.DataContext = new FinOpsReportProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is FinOpsReportProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}