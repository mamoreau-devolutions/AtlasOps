namespace AtlasOps.Features.Api.ApiEndpointRecovery;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiEndpointRecoveryView : UserControl
{
    public ApiEndpointRecoveryView()
    {
        this.DataContext = new ApiEndpointRecoveryViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiEndpointRecoveryViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}