namespace AtlasOps.Features.Api.ApiGatewayProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiGatewayProvisioningView : UserControl
{
    public ApiGatewayProvisioningView()
    {
        this.DataContext = new ApiGatewayProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiGatewayProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}