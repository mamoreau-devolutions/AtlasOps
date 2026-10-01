namespace AtlasOps.Features.Api.ApiClientProvisioning;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

public sealed partial class ApiClientProvisioningView : UserControl
{
    public ApiClientProvisioningView()
    {
        this.DataContext = new ApiClientProvisioningViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    private void Advance_OnClick(object? sender, RoutedEventArgs e)
    {
        if (this.DataContext is ApiClientProvisioningViewModel viewModel)
        {
            viewModel.Advance();
        }
    }
}