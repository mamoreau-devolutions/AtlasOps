namespace AtlasOps.Features.Api.ApiDeploymentProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiDeploymentProvisioningView : UserControl
{
    public ApiDeploymentProvisioningView()
    {
        this.DataContext = new ApiDeploymentProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiDeploymentProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}