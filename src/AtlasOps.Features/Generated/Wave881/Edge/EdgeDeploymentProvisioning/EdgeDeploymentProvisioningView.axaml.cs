namespace AtlasOps.Features.Edge.EdgeDeploymentProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class EdgeDeploymentProvisioningView : UserControl
{
    public EdgeDeploymentProvisioningView()
    {
        this.DataContext = new EdgeDeploymentProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is EdgeDeploymentProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}