namespace AtlasOps.Features.Api.ApiEndpointGovernance;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiEndpointGovernanceView : UserControl
{
    public ApiEndpointGovernanceView()
    {
        this.DataContext = new ApiEndpointGovernanceViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiEndpointGovernanceViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}