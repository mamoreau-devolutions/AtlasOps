namespace AtlasOps.Features.Api.ApiGatewayRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiGatewayRecoveryView : UserControl
{
    public ApiGatewayRecoveryView()
    {
        this.DataContext = new ApiGatewayRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiGatewayRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}