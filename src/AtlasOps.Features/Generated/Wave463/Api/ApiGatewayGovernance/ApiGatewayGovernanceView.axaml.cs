namespace AtlasOps.Features.Api.ApiGatewayGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiGatewayGovernanceView : UserControl
{
    public ApiGatewayGovernanceView()
    {
        this.DataContext = new ApiGatewayGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiGatewayGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}