namespace AtlasOps.Features.Api.ApiEndpointProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiEndpointProvisioningView : UserControl
{
    public ApiEndpointProvisioningView()
    {
        this.DataContext = new ApiEndpointProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiEndpointProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}